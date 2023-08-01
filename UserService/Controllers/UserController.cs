using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Validators;

namespace UserService.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserContext _database;

        public UserController(ILogger<UserController> logger, UserContext database)
        {
            _logger = logger;
            _database = database;
        }

        private static string EncryptPassword(string passsword)
        {
            byte[] byteSalt = Convert.FromBase64String("412G3AS/jBnm6bbgZp4LWw==");

            string hashedpw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: passsword,
                salt: byteSalt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 64
                ));

            return hashedpw;
        }

        [HttpPost(Name = "Register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] NewUser user)
        {
            bool validationResult = await UserValidator.ValidateUserData(user, _database);
            if (!validationResult) 
            { 
                return ValidationProblem("Unable to register new user with given data.");
            }

            User newUser = Models.User.GenerateFrom(user);

            await _database.Users.AddAsync(newUser);

            var newUserCredentials = new UserCredentials()
            {
                Login = user.Login,
                Password = EncryptPassword(user.Password),
                UserData = newUser
            };

            await _database.UserCredentials.AddAsync(newUserCredentials);

            await _database.SaveChangesAsync();
            return Ok("User registered succesfully.");
        }
    }
}