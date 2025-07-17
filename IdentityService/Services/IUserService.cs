using Common;
using IdentityService.Models;

namespace IdentityService.Services;

public interface IUserService
{
  Task<ServiceResult> RegisterNewUser(NewUser registrationData, CancellationToken cancellationToken = default);
  Task<ServiceResult<string>> LogInUser(UserCredentials credentials, CancellationToken cancellationToken = default);
  Task<ServiceResult<string>> GetUsernameForLoggedUser(Guid userId, CancellationToken cancellationToken = default);
}
