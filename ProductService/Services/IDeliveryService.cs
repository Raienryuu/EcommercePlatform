using Common;
using ProductService.Models;

namespace ProductService.Services;

public interface IDeliveryService
{
  Task<ServiceResult<List<Delivery>>> GetAvailableDeliveries();
}
