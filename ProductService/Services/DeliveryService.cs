using Common;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Services;

public class DeliveryService(ProductDbContext productDb) : IDeliveryService
{
  public async Task<ServiceResult<List<Delivery>>> GetAvailableDeliveries()
  {
    var deliveries = await productDb.Deliveries.AsNoTracking().ToListAsync();
    return ServiceResults.Ok(deliveries);
  }
}
