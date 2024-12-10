using ProductService.Utility.OrderSetters;

namespace ProductService.Models;

public class PaginationParams
{
  public enum SortType
  {
	PriceAsc = 1,
	PriceDesc = 2,
	QuantityAsc = 3
  }
  public int PageNum { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Name { get; set; }
  public decimal? MinPrice { get; set; }
  public decimal? MaxPrice { get; set; }
  public int? MinQuantity { get; set; }

  public IOrderable<int>? Categories { get; set; }

  public SortType Order { get; set; } = SortType.PriceAsc;

}