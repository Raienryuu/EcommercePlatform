using System.Collections.Generic;
using System.Linq;
using MessageQueue.DTOs;

namespace OrderService.Models;

public static class OrderProductExtensions
{
    public static OrderProductDTO[] ToOrderProductDTOArray(this ICollection<OrderProduct> products)
    {
        return products.Select(p => new OrderProductDTO
        {
            ProductId = p.ProductId,
            Quantity = p.Quantity,
            Price = p.Price
        }).ToArray();
    }
}