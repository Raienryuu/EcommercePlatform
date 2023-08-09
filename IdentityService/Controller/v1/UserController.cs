using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityService.Controller
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;
        public UserController(ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager) { 
            _db = db;
            _userManager = userManager;
            _signinManager = signInManager;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegisterNewUser([FromBody] NewUser registrationData)
        {
            PasswordHasher<IdentityUser>  passwordHasher = new();
            //data validation
            IdentityUser newUser = new(){
                UserName = registrationData.UserName,
                Email = registrationData.Email,
                PhoneNumber = registrationData.PhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
            };

            newUser.PasswordHash = passwordHasher.HashPassword(newUser, registrationData.Password);
            var identityResult = await _userManager.CreateAsync(newUser);

            if (identityResult.Succeeded) {
                UserAddress userAddress = UserAddress.CreateFrom(registrationData, newUser);
                await _db.Addresses.AddAsync(userAddress);
                await _db.SaveChangesAsync();
                return Ok();
            } else {
                return BadRequest("Invalid user data");
            }
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<JwtSecurityToken> > Login(
            [FromBody] UserCredentials credentials)
        {
            if (credentials is null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            var user = await _userManager.FindByNameAsync(credentials.Login);
            if (user is not null){
                var result = await _signinManager.PasswordSignInAsync(user, credentials.Password, false, false);
                if(result.Succeeded)
                {
                    return Accepted();
                }
            }

            return BadRequest("{\"message\":\"Not found matching data in database.\"}");
        }

        // PUT api/<AuthorizationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthorizationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
