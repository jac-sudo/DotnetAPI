using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {

        DataContextEF _entityFramework;
        IUserRepository _userRepository;
        IMapper _mapper;

        public UserEFController(IConfiguration config, IUserRepository userRepository)
        {
            _entityFramework = new DataContextEF(config);
            _userRepository = userRepository;

            _mapper = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<UserDto, User>();
            }));
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers() {

            IEnumerable<User> users = _userRepository.GetUsers();

            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId) {
            
            return _userRepository.GetSingleUser(userId);
        }


        [HttpPut("EditUser")]
        public IActionResult EditUser(User user) {
            
            User? userDb = _userRepository.GetSingleUser(user.UserId);

            if(userDb != null) {
                userDb.Active = user.Active;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Gender = user.Gender;
            }

            if(_userRepository.SaveChanges())
                return Ok();

            throw new Exception("Impossibile aggiornare l'utente");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto user) {

            User userDb = _mapper.Map<User>(user);

                _userRepository.AddEntity<User>(userDb);

            if(_userRepository.SaveChanges())
                return Ok();

            throw new Exception("Impossibile aggiungere l'utente");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId) {

            User? userDb = _entityFramework.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefault<User>();

            if(userDb != null) {

                _entityFramework.Users.Remove(userDb);

                if(_userRepository.SaveChanges())
                    return Ok();
            }

            throw new Exception("Impossibile eliminare l'utente");
        }
    }
}