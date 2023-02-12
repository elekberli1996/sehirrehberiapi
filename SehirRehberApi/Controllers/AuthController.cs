using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SehirRehberApi.Data;
using SehirRehberApi.Dto;
using SehirRehberApi.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SehirRehberApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAutRepository _outRepository;
        IConfiguration _configuration;

        public AuthController(IAutRepository outRepository, IConfiguration configuration)
        {
            _outRepository = outRepository;
            _configuration = configuration;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            if (await _outRepository.UserExsists(userForRegisterDto.UserName))
            {
                ModelState.AddModelError("UserName", "Username alredy exstis");

            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User newUser = new User { Username = userForRegisterDto.UserName };

           var createdUser= await _outRepository.Register(newUser, userForRegisterDto.Password);


            return StatusCode(201,createdUser);

        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] UserForLoginDto userForLoginDto)
        {
            var user = _outRepository.Login(userForLoginDto.UserName, userForLoginDto.Password);

            if (user==null)
            {
                return Unauthorized();

            }
            // bir token ureticez

            var tokenHandler = new JwtSecurityTokenHandler();// tokenin isini kim yapicak

            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSetting:Token").Value);
            //hangi anahtara gore token ureticek

            var tokenDescriptor = new SecurityTokenDescriptor
            {// token neleri tutacak 
                Subject = new ClaimsIdentity(new Claim[] {

                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)

                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            string JsonToken = JsonConvert.SerializeObject(tokenString);

          
            return Ok(JsonToken);






        }



    }
}
