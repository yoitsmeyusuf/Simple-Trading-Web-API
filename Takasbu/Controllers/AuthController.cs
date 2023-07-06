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

         private readonly ProductService _ProductService;

        public static User user = new User();
        public static Product product = new Product();
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration, UsersService UsersService,AuthService authService,ProductService ProductService)
        {  
           _ProductService = ProductService;
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



          [HttpPut("Updatebio")]
        public async Task<ActionResult<string>> biochange(string username,string newbio)
        {
            if(aut(username)){
             var users = await _UsersService.GetAsync();

            var user = users.FirstOrDefault(u => u.Username == username);
            user!.Bio = newbio;
            Console.WriteLine(user!.Bio);
            await _UsersService.UpdateAsyncu(user.Username,user);

           

            return Ok();

            }
             return BadRequest();

         
        }
    
       [HttpPost("CreateProduct")]
        public async Task<ActionResult<string>> CreateProduct(ProductDTO request)
        {
              var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
              var token = _AuthService.GetUserIdFromToken(_bearer_token!);
               var users = await _UsersService.GetAsync();
            var user = users.FirstOrDefault(u => u.Username == token);
              if(token==null|| user==null){
                return BadRequest("Token or User cant found");

              }
             product.Name=request.Name;
             product.Category = request.Category;
             product.Description = request.Description;
             product.Price = request.Price;
             product.Owner = token;
              await _ProductService.CreateAsync(product);
             user.ProductIds.Add(product.Id);
             await _UsersService.UpdateAsync(user.Id,user);

        

            return Ok("created");
        }
        [HttpGet("GetProducts")]
        public async Task<ActionResult<string>> GeTProducts()
        {
              var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
              var token = _AuthService.GetUserIdFromToken(_bearer_token!);
               var users = await _UsersService.GetAsync();
            var user = users.FirstOrDefault(u => u.Username == token);
              if(token==null|| user==null){
                return BadRequest("Token or User cant found");

              }
          var result =  await _ProductService.GetListAsync(user.ProductIds);

        

            return Ok(result);
        }


       



        //private methods
        private bool aut(string UserName)
        {
               var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
               var token = _AuthService.GetUserIdFromToken(_bearer_token!);
              
              if(UserName==token){
                return true;
              }
            
            return false;
                  
        }

    }
}