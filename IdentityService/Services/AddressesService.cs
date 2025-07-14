using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;
using Common;

namespace IdentityService.Services;

public class AddressesService(ApplicationDbContext db) : IAddressesService
{
  private readonly ApplicationDbContext _db = db;

  public async Task<ServiceResult<UserAddress>> AddAddressAsync(UserAddress address)
  {
    address.Id = 0;
    _db.Addresses.Add(address);
    await _db.SaveChangesAsync();
    return ServiceResults.Ok(address);
  }

  public async Task<ServiceResult<UserAddress>> EditAddressAsync(UserAddress newAddress)
  {
    var oldAddress = await _db.Addresses.FindAsync(newAddress.Id);
    if (oldAddress == null)
      return ServiceResults.NotFound<UserAddress>("Address not found");

    var tempId = oldAddress.Id;
    oldAddress = newAddress;
    oldAddress.Id = tempId;

    await _db.SaveChangesAsync();
    return ServiceResults.Ok(oldAddress);
  }

  public async Task<ServiceResult<UserAddress>> GetAddressAsync(int addressId)
  {
    var address = await _db.Addresses.FindAsync(addressId);

    if (address is null)
    {
      return ServiceResults.NotFound<UserAddress>("Address not found");
    }

    return ServiceResults.Ok(address);
  }

  public async Task<List<UserAddress>> GetAddressesForUserAsync(Guid userId)
  {
    return await _db.Addresses.Where(x => x.UserId == userId).ToListAsync();
  }

  public async Task<ServiceResult> RemoveAddressAsync(int addressId)
  {
    var address = await _db.Addresses.FindAsync(addressId);

    if (address is null)
    {
      return ServiceResults.Error("Address not found",404);
    }

    _db.Addresses.Remove(address);
    await _db.SaveChangesAsync();

    return ServiceResults.Success(200);
  }
}
