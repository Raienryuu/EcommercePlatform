using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.Unit.Stubs;

public class UserManagerStub<IdenitityUser> : UserManager<IdenitityUser>
{
    public UserManagerStub(IUserStore<IdenitityUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<IdenitityUser> passwordHasher, IEnumerable<IUserValidator<IdenitityUser>> userValidators, IEnumerable<IPasswordValidator<IdenitityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<IdenitityUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }
}