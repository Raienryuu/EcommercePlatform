using System.Reflection;

namespace IdentityService.Models.Validators
{
    public class NewUserValidator : IValidatable
    {
        public bool Validate(in IUserData? o)
        {
            if (o is null) { return false; }
            foreach (PropertyInfo prop in typeof(NewUser).GetProperties())
            {
                if(prop.GetValue(o) is null)
                {
                    return false;
                }
            }
            //more specified validation
            return true;
        }

    }
}
