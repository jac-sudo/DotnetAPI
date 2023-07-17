using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController : ControllerBase
    {

        DataContextDapper _dapper;

        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection() {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId) {
            
            string sql = @"SELECT [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active] FROM TutorialAppSchema.Users
                        WHERE UserId = " + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
            return user;
        }

        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId, bool isActive) {

            string sql = @"EXEC TutorialAppSchema.spUsers_Get";

            string parameters = "";
            
            if(userId != 0) {
                parameters += ", @UserId =" + userId.ToString();
            }

            if(isActive) {
                parameters += ", @Active =" + isActive.ToString();
            }

            sql += parameters.Substring(1);

            IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserComplete user) {
            
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                @FirstName = '" + user.FirstName + 
                "', @LastName = '" + user.LastName + 
                "', @Email = '" + user.Email + 
                "', @Gender = '" + user.Gender +
                "', @Active = '" + user.Active +
                "', @JobTitle = '" + user.JobTitle +
                "', @Department = '" + user.Department +
                "', @Salary = '" + user.Salary +
                "', @UserId = " + user.UserId;

                Console.WriteLine(sql);

            if(_dapper.ExecuteSql(sql))
                return Ok();

            throw new Exception("Impossibile aggiornare l'utente");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId) {
            string sql = @"TutorialAppSchema.spUser_Delete
                        WHERE UserId =" + userId.ToString();

            Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql))
                    return Ok();

            throw new Exception("Impossibile eliminare l'utente");
        }
    }
}