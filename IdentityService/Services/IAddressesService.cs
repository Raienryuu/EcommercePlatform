using Common;
using IdentityService.Models;

namespace IdentityService.Services;

public interface IAddressesService
{
  public Task<List<UserAddress>> GetAddressesForUserAsync(Guid userId, CancellationToken cancellationToken = default);
  public Task<ServiceResult<UserAddress>> AddAddressAsync(UserAddress address, CancellationToken cancellationToken = default);
  public Task<ServiceResult> RemoveAddressAsync(int addressId, CancellationToken cancellationToken = default);
  public Task<ServiceResult<UserAddress>> EditAddressAsync(UserAddress address, CancellationToken cancellationToken = default);
  public Task<ServiceResult<UserAddress>> GetAddressAsync(int addressId, CancellationToken cancellationToken = default);
}
