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

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user) {
            
            string sql = @"
                UPDATE TutorialAppSchema.Users
                SET [FirstName] = '" + user.FirstName + 
                    "', [LastName] = '" + user.LastName + 
                    "', [Email] = '" + user.Email + 
                    "', [Gender] = '" + user.Gender + 
                    "', [Active] = '" + user.Active + 
                "' WHERE UserId = " + user.UserId;

                Console.WriteLine(sql);

            if(_dapper.ExecuteSql(sql))
                return Ok();

            throw new Exception("Impossibile aggiornare l'utente");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto user) {

            string sql = @"
            INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES (" +
                "'" + user.FirstName + 
                "','" + user.LastName + 
                "','" + user.Email +
                "','" + user.Gender +
                "','" + user.Active +
            "')";
            
            Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql))
                    return Ok();

            throw new Exception("Impossibile aggiungere l'utente");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId) {
            string sql = @"DELETE FROM TutorialAppSchema.Users 
                        WHERE UserId =" + userId.ToString();

            Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql))
                    return Ok();

            throw new Exception("Impossibile eliminare l'utente");
        }

        [HttpPost("UserSalary")]
        public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.UserSalary (
                    UserId,
                    Salary
                ) VALUES (" + userSalaryForInsert.UserId.ToString()
                    + ", " + userSalaryForInsert.Salary
                    + ")";

            if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
            {
                return Ok(userSalaryForInsert);
            }
            throw new Exception("Adding User Salary failed on save");
        }

        [HttpPut("UserSalary")]
        public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary=" 
                + userSalaryForUpdate.Salary
                + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userSalaryForUpdate);
            }
            throw new Exception("Updating User Salary failed on save");
        }

        [HttpDelete("UserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId=" + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Deleting User Salary failed on save");
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
        {
            string sql = @"
                INSERT INTO TutorialAppSchema.UserJobInfo (
                    UserId,
                    Department,
                    JobTitle
                ) VALUES (" + userJobInfoForInsert.UserId
                    + ", '" + userJobInfoForInsert.Department
                    + "', '" + userJobInfoForInsert.JobTitle
                    + "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForInsert);
            }
            throw new Exception("Adding User Job Info failed on save");
        }

        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='" 
                + userJobInfoForUpdate.Department
                + "', JobTitle='"
                + userJobInfoForUpdate.JobTitle
                + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForUpdate);
            }
            throw new Exception("Updating User Job Info failed on save");
        }
        
        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"
                DELETE FROM TutorialAppSchema.UserJobInfo 
                    WHERE UserId = " + userId.ToString();
            
            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            } 

            throw new Exception("Failed to Delete User");
        }
    }
}