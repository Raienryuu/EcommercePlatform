using IdentityService.Models;
using IdentityService.Services;
using IdentityService.Tests.Fakes;
using IdentityService.Tests.SampleData;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Tests;

public class AddressServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly AddressesService _addresses;
  private readonly ApplicationDbContextFake _dbContext;

  public AddressServiceTests(DatabaseFixture databaseFixture)
  {
    _dbContext = new ApplicationDbContextFake(
      new DbContextOptionsBuilder<Data.ApplicationDbContext>()
        .UseSqlServer(databaseFixture.GetConnectionString())
        .Options
    );
    _addresses = new AddressesService(_dbContext);
  }

  [Fact]
  public async Task AddAddress_ValidAddress_AddressWithGivenId()
  {
    var aliceGuid = Guid.Parse("D0D480DA-1E50-4A78-A727-600D986D8075");
    var aliceUser = await _dbContext.Users.FindAsync(aliceGuid);

    var newAddress = new UserAddress
    {
      Address = "Willow St",
      City = "Chicago",
      Country = "United States of America",
      Email = "toomfoolery@mail.com",
      FullName = "Thomas Fool",
      PhoneNumber = "+132132412312",
      ZIPCode = "1423-1234",
      UserId = aliceUser!.Id,
    };

    var addedObject = await _addresses.AddAddressAsync(newAddress);

    Assert.True(addedObject.Value.Id > 0);
  }

  [Fact]
  public async Task GetAddress_ExistentId_Address()
  {
    const int ADDRESS_ID = 1;
    var aliceGuid = Guid.Parse("D0D480DA-1E50-4A78-A727-600D986D8075");
    const string ALICE_PHONE = "+1324876582";

    var address = await _addresses.GetAddressAsync(ADDRESS_ID);

    Assert.True(address.Value.PhoneNumber == ALICE_PHONE);
    Assert.True(address.Value.UserId == aliceGuid);
  }

  [Fact]
  public async Task GetAddressesForUser_ExistentGuid_CollectionOfAddresses()
  {
    var aliceGuid = Guid.Parse("D0D480DA-1E50-4A78-A727-600D986D8075");

    var addresses = await _addresses.GetAddressesForUserAsync(aliceGuid);

    Assert.NotEmpty(addresses);
  }

  [Fact]
  public async Task RemoveAddress_ExistentGuid_AmountOfUserAddressesDecremented()
  {
    var aliceGuid = Guid.Parse("D0D480DA-1E50-4A78-A727-600D986D8075");
    var newAddress = SampleUserData.SampleAddress;
    const string CITY = "Removado";
    newAddress.UserId = aliceGuid;
    newAddress.City = CITY;
    await _addresses.AddAddressAsync(newAddress);
    var addresses = await _addresses.GetAddressesForUserAsync(aliceGuid);
    var addressesCountBeforeDelete = addresses.Count;
    var idToDelete = addresses.First(a => a.City == CITY).Id;

    await _addresses.RemoveAddressAsync(idToDelete);

    addresses = await _addresses.GetAddressesForUserAsync(aliceGuid);
    var addressesCountAfterDelete = addresses.Count;
    Assert.Equal(addressesCountBeforeDelete, addressesCountAfterDelete + 1);
  }

  [Fact]
  public async Task EditAddress_AddressWithNewValue_UpdatedAddress()
  {
    var aliceGuid = Guid.Parse("D0D480DA-1E50-4A78-A727-600D986D8075");
    var oldAddress = SampleUserData.SampleAddress;
    const string CITY = "Edition";
    oldAddress.UserId = aliceGuid;
    oldAddress.City = CITY;
    await _addresses.AddAddressAsync(oldAddress);
    oldAddress.City = "Mexico";

    var updatedAddress = await _addresses.EditAddressAsync(oldAddress);

    Assert.NotEqual(CITY, updatedAddress.Value.City);
  }
}
