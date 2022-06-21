using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServerApp.DTO;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDto dto)
        {

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Gender = dto.Gender,
                UserName = dto.UserName,
                Created = DateTime.Now,
                LastActive = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserForRegisterDTO dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Gender = dto.Gender,
                UserName = dto.UserName,
                Created = DateTime.Now,
                LastActive = DateTime.Now,
                DateOfBirth=dto.DateOfBirth,
                City=dto.City,
                Country=dto.Country,
                
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                return StatusCode(201);
            }
            else
            {
                return BadRequest(result.Errors);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserForLoginDTO model)
        {

            // throw new Exception("internal Exception-Zeki Server");
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null)
                return BadRequest(new { message = "User Not Found!!" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {

                return Ok(new
                {
                    token = GenerateToken(user),
                    userName = user.UserName
                });
            }
            else
            {
                return Unauthorized();
            }


        }

        [HttpGet]
        private string GenerateToken(User user)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                                     new Claim(ClaimTypes.Name,user.UserName.ToString()),
                                                     new Claim("email",user.Email.ToString())}),
                Expires = DateTime.UtcNow.AddDays(100),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}