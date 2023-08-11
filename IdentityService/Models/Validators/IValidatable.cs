namespace IdentityService.Models.Validators
{
    public interface IValidatable
    {
        public bool Validate(in IUserData? o);
    }
}
