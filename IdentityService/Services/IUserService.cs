using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public interface IUserService
{
  Task<(bool isSucccess, IEnumerable<IdentityError> errorList)> RegisterNewUser(NewUser registrationData);
  Task<(bool isSuccess, string message)> LogInUser(UserCredentials credentials);
  Task<(bool isSuccess, string username)> GetUsernameForLoggedUser(Guid userId);
}
