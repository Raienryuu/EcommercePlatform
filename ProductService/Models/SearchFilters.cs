namespace ProductService.Models;

public class SearchFilters
{
  public enum SortType
  {
    MinPrice = 1,
    MaxPrice = 2
  }
  public string? Name { get; set; }
  public decimal? MinPrice { get; set; }
  public decimal? MaxPrice { get; set; }
  public int? MinQuantity { get; set; }

  public SortType Order { get; set; } = SortType.MinPrice;

  public SearchFilters(){}

  public SearchFilters(SearchFilters filters)
  {
    Name = filters.Name;
    MinPrice = filters.MinPrice;
    MaxPrice = filters.MaxPrice;
    MinQuantity = filters.MinQuantity;
    Order = filters.Order;
  }
}