using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IdentityService.Services;

public interface IAddressesService
{
  public Task<List<UserAddress>> GetAddressesForUserAsync(Guid userId);
  public Task<UserAddress> AddAddressAsync(UserAddress address);
  public Task RemoveAddressAsync(int addressId);
  public Task<UserAddress?> EditAddressAsync(UserAddress address);
  public Task<UserAddress?> GetAddressAsync(int addressId);
}
