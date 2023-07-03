using Takasbu.Models;
using Takasbu.Models.DTO;
using Microsoft.AspNetCore.Http;
using Takasbu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;

//TODO services eklicez UNUTMA YUSUF 
namespace Takasbu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {


       private readonly AuthService _AuthService;
        private readonly UsersService _UsersService;

        public static User user = new User();
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration, UsersService UsersService,AuthService authService)
        {
            _AuthService = authService;
            _UsersService = UsersService;
            _configuration = configuration;
        }
        public async Task<List<User>> Get() =>
              await _UsersService.GetAsync();


        [HttpPost("register")]
        public async Task<ActionResult<User>> Registration(UserDto request)
        {

            var users = await _UsersService.GetAsync();
            var userg = users.FirstOrDefault(u => u.Username == request.Username);

            if(userg==null){
               _AuthService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _UsersService.CreateAsync(user);
            return Ok(user);
            }
          return  BadRequest("There is a same Username with a user");
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginAsync(UserDto request)
        {
            var users = await _UsersService.GetAsync();

            var user = users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null)
                return BadRequest("User not Found");

            if (!_AuthService.VerifyPasswordHash(request.Password, user.PasswordHash!, user.PasswordSalt!))
                return BadRequest("Incorrect Username or Password!");

            var token = _AuthService.GenerateToken(user);

            return Ok(token);
        }
        [HttpPost("something")]
        public async Task<ActionResult<string>> something(string a)
        {
               var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
               var token = _AuthService.GetUserIdFromToken(_bearer_token!);
              var user = await _UsersService.GetAsyncu(token!);
              
              Console.WriteLine(_bearer_token);


            return Ok(user);
        }



        //private methods

    }
}