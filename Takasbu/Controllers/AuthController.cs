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
        public AuthController(IConfiguration configuration, UsersService UsersService, AuthService authService, ProductService ProductService)
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

            if (userg == null)
            {
                _AuthService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.Username = request.Username;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await _UsersService.CreateAsync(user);
                return Ok(user);
            }
            return BadRequest("There is a same Username with a user");
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
        public async Task<ActionResult<string>> biochange(string username, string newbio)
        {
            if (aut(username))
            {
                var users = await _UsersService.GetAsync();

                var user = users.FirstOrDefault(u => u.Username == username);
                user!.Bio = newbio;
                Console.WriteLine(user!.Bio);
                await _UsersService.UpdateAsync(user.Username, user, true);



                return Ok();

            }
            return BadRequest();


        }

        [HttpPost("CreateProduct")]
        public async Task<ActionResult<string>> CreateProduct(ProductDTO request, IFormFile file)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);
            if (token == null)
            {
                return BadRequest("Token  cant found");
            }
            var user = await _UsersService.GetAsync(token, true);
            if (user == null)
            {
                return BadRequest("User cant found");

            }

            if (file != null && file.Length > 0)
            {
                // Generate a unique file name or use the original file name
                string originalFileName = file.FileName; // Assuming you have the original file name
                string extension = Path.GetExtension(originalFileName);
                string fileName = Guid.NewGuid().ToString() + extension;

                // Set the file path where you want to save the uploaded files
                string filePath = Path.Combine(@"C:\pictures", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                product.Picture = fileName;
            }
            product.Name = request.Name;
            product.Category = request.Category;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Owner = token;
            await _ProductService.CreateAsync(product);
            user.ProductIds.Add(product.Id);
            await _UsersService.UpdateAsync(user.Id, user);



            return Ok("created");
        }


        [HttpGet("GetProducts")]
        public async Task<ActionResult<string>> GeTProducts()
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);
            if (token == null) { return BadRequest("Token cant found"); }
            var user = await _UsersService.GetAsync(token, true);

            if (user == null)
            {
                return BadRequest("User cant found");

            }

            var result = await _ProductService.GetListAsync(user.ProductIds);



            return Ok(result);
        }


        [HttpPut("UpdateSelf")]
        public async Task<IActionResult> Update(string UserName, User updatedUser)
        {
            if (aut(UserName))
            {
                var User = await _UsersService.GetAsync(UserName, true);

                if (User is null)
                {
                    return NotFound();
                }

                updatedUser.Id = User.Id;

                await _UsersService.UpdateAsync(UserName, updatedUser, true);

                return NoContent();


            }
            return BadRequest("Token or User cant found");


        }






        //private methods
        private bool aut(string UserName)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);

            if (UserName == token)
            {
                return true;
            }

            return false;

        }

    }
}