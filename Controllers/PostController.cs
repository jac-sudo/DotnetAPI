using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config) {

            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None") {

            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";

            if(postId != 0) {
                parameters += ", @PostId =" + postId.ToString();
            }
            if(userId != 0) {
                parameters += ", @UserId =" + userId.ToString();
            }
            if(searchParam.ToLower() != "none") {
                parameters += ", @SearchValue ='" + searchParam + "'";
            }

            if(parameters.Length > 0)
                sql += parameters.Substring(1);

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts() {

            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = '" +
                this.User.FindFirst("userId")?.Value.ToString() + "'";

            Console.WriteLine(sql);

            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]) VALUES ('" + this.User.FindFirst("userId")?.Value.ToString()
                + "','" + postToAdd.PostTitle
                + "','" + postToAdd.PostContent
                + "', GETDATE(), GETDATE() )";
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post!");
        }


        //[AllowAnonymous]
        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert) {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId = '" + this.User.FindFirst("userId")?.Value.ToString() +
                "', @PostTitle = '" + postToUpsert.PostTitle +
                "', @PostContent = '" + postToUpsert.PostContent + "'";

            if(postToUpsert.PostId > 0)
                sql += ", @PostId = " + postToUpsert.PostId;

            if(_dapper.ExecuteSql(sql)) {
                return Ok();
            }

            throw new Exception("Impossibile modificare il post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId) {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = '" +
                postId.ToString() +
                    "', @UserId = '" + this.User.FindFirst("userId")?.Value.ToString() + "'";

            if(_dapper.ExecuteSql(sql)) {
                return Ok();
            }

            throw new Exception("Impossibile eliminare il post");
        }
    }
}