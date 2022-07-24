using AuthenticationService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthenticationService.Controllers
{
    public class AuthController : Controller
    {
        private readonly busticketdbContext context;
        private readonly IConfiguration _configuration;

        public AuthController(busticketdbContext _dbcontext, IConfiguration configuration) {
            _configuration = configuration;
            context = _dbcontext;
        }

        [HttpPost]
        [Route("/api/login")]
        public async Task<ActionResult<Userdatum>> Login([FromBody] Login authDetails)
        {
            var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.EmailId == authDetails.EmailId);
            if (userFound == null)
            {
                return BadRequest("User not found");
            }
            else if(userFound.Password != authDetails.Password)
            {
                return BadRequest("Invalid Email or Password");
            }
            else
            {
                string token = CreateToken(userFound);
                return Ok(token);
            }
            
        }

        private string CreateToken(Userdatum user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id",""+user.UserId),
                new Claim(ClaimTypes.Role, ""+user.IsAdmin)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWT:Secret").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<ActionResult<Userdatum>> Register([FromBody] Userdatum user)
        {
            var userFound = await context.Userdata.FirstOrDefaultAsync(x => x.EmailId == user.EmailId);
            if (userFound != null)
            {
                return BadRequest("User with this email exists already!");

            }
            else
            {
                
                context.Userdata.Add(user);
                await context.SaveChangesAsync();
                return Ok("User Registered Successfully");

            }
           
        }

    }
}
