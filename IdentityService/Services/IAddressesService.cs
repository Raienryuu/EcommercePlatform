using Common;
using IdentityService.Models;

namespace IdentityService.Services;

public interface IAddressesService
{
  public Task<List<UserAddress>> GetAddressesForUserAsync(Guid userId);
  public Task<ServiceResult<UserAddress>> AddAddressAsync(UserAddress address);
  public Task<ServiceResult> RemoveAddressAsync(int addressId);
  public Task<ServiceResult<UserAddress>> EditAddressAsync(UserAddress address);
  public Task<ServiceResult<UserAddress>> GetAddressAsync(int addressId);
}
