using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class AddressesService(ApplicationDbContext db) : IAddressesService
{
  private readonly ApplicationDbContext _db = db;

  public async Task<UserAddress> AddAddressAsync(UserAddress address)
  {
    address.Id = default;
    await _db.Addresses.AddAsync(address);
    await _db.SaveChangesAsync();
    return address;
  }

  public async Task<UserAddress?> EditAddressAsync(UserAddress newAddress)
  {
    var oldAddress = await _db.Addresses.FindAsync(newAddress.Id);
    if (oldAddress == null)
      return default;

    var idInDb = oldAddress.Id;
    oldAddress = newAddress;
    oldAddress.Id = idInDb;

    await _db.SaveChangesAsync();
    return oldAddress;
  }

  public async Task<UserAddress?> GetAddressAsync(int addressId)
  {
    return await _db.Addresses.FindAsync(addressId);
  }

  public async Task<List<UserAddress>> GetAddressesForUserAsync(Guid userId)
  {
    return await _db.Addresses.Where(x => x.UserId == userId).ToListAsync();
  }

  public async Task RemoveAddressAsync(int addressId)
  {
    await _db.Addresses.Where(x => x.Id == addressId).ExecuteDeleteAsync();
  }
}
