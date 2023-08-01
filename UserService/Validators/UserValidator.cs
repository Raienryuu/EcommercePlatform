using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Validators
{
    public static class UserValidator
    {
        public async static Task<bool> ValidateUserData(NewUser user, UserContext _database)
        {
            if (user is null) return false;
            if (!ValidateCredentials(user)) return false;
            if (!ValidateUserInformation(user)) return false;
            // do a lot of validation
            var loginTaken = await _database.UserCredentials.FirstAsync(x => x.Login == user.Login);
            if (loginTaken is not null) return false;
            return true;
        }

        private static bool ValidateUserInformation(NewUser user)
        {
            throw new NotImplementedException();
        }

        private static bool ValidateCredentials(NewUser user)
        {
            throw new NotImplementedException();
        }
    }
}
