using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config) {

            _dapper = new DataContextDapper(config);
        }

        [HttpGet("PostSingle/{postId}")]
        public IEnumerable<Post> GetPosts(int postId) {

            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] FROM TutorialAppSchema.Posts 
                        WHERE PostId = '" + postId.ToString() + "'";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId) {

            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] FROM TutorialAppSchema.Posts 
                        WHERE PostId = '" + userId.ToString() + "'";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts() {

            string sql = @"
                SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE PostId = " + User.FindFirst("userId")?.Value;


            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd) {
            string sql = @"
            INSERT INTO TutorialAppSchema.Posts (
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]
            ) VALUES (" + User.FindFirst("userId")?.Value +
            ", '" + postToAdd.PostTitle +
            ", '" + postToAdd.PostContent +
            "', GETDATE(), GETDATE())";

            if(_dapper.ExecuteSql(sql)) {
                return Ok();
            }

            throw new Exception("Impossibile creare un nuovo post");
        }

        //[AllowAnonymous]
        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit) {
            string sql = @"
            UPDATE TutorialAppSchema.Posts SET
                PostContent = '" + postToEdit.PostContent +
             "', PostTitle = '" + postToEdit.PostTitle +
             @"', PostUpdated = GETDATE()
            WHERE PostId = " + postToEdit.PostId.ToString() +
            "AND UserId = " + this.User.FindFirst("userId")?.Value;

            if(_dapper.ExecuteSql(sql)) {
                return Ok();
            }

            throw new Exception("Impossibile modificare il post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId) {
            string sql = "DELETE FROM TutorialAppSchema.Posts WHERE PostId =" + postId.ToString();

            if(_dapper.ExecuteSql(sql)) {
                return Ok();
            }

            throw new Exception("Impossibile eliminare il post");
        }
    }
}