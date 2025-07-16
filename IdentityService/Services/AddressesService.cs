using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;
using Common;

namespace IdentityService.Services;

public class AddressesService(ApplicationDbContext db) : IAddressesService
{
  private readonly ApplicationDbContext _db = db;

  public async Task<ServiceResult<UserAddress>> AddAddressAsync(UserAddress address, CancellationToken cancellationToken = default)
  {
    address.Id = 0;
    _db.Addresses.Add(address);
    await _db.SaveChangesAsync(cancellationToken);
    return ServiceResults.Ok(address);
  }

  public async Task<ServiceResult<UserAddress>> EditAddressAsync(UserAddress newAddress, CancellationToken cancellationToken = default)
  {
    var oldAddress = await _db.Addresses.FindAsync(newAddress.Id, cancellationToken);
    if (oldAddress == null)
      return ServiceResults.NotFound<UserAddress>("Address not found");

    var tempId = oldAddress.Id;
    oldAddress = newAddress;
    oldAddress.Id = tempId;

    await _db.SaveChangesAsync(cancellationToken);
    return ServiceResults.Ok(oldAddress);
  }

  public async Task<ServiceResult<UserAddress>> GetAddressAsync(int addressId, CancellationToken cancellationToken = default)
  {
    var address = await _db.Addresses.FindAsync(addressId, cancellationToken);

    if (address is null)
    {
      return ServiceResults.NotFound<UserAddress>("Address not found");
    }

    return ServiceResults.Ok(address);
  }

  public async Task<List<UserAddress>> GetAddressesForUserAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return await _db.Addresses.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
  }

  public async Task<ServiceResult> RemoveAddressAsync(int addressId, CancellationToken cancellationToken = default)
  {
    var address = await _db.Addresses.FindAsync(addressId, cancellationToken);

    if (address is null)
    {
      return ServiceResults.Error("Address not found",404);
    }

    _db.Addresses.Remove(address);
    await _db.SaveChangesAsync(cancellationToken);

    return ServiceResults.Success(200);
  }
}
