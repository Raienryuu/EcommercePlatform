using Common;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public interface IUserService
{
  Task<ServiceResult> RegisterNewUser(NewUser registrationData);
  Task<ServiceResult<string>> LogInUser(UserCredentials credentials);
  Task<ServiceResult<string>> GetUsernameForLoggedUser(Guid userId);
}
