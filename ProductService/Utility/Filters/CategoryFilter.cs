using System.Linq.Expressions;
using ProductService.Models;

namespace ProductService.Utility.Filters;

public class CategoryFilter : IFilterable<Product>
{
  public object? FilterValue { get; set; }
  public bool IsMainFilter { get; set; }
  public IQueryable<Product> ApplyFilterForOffsetPage(IQueryable<Product> query)
  {
    throw new NotImplementedException();
  }

  public Expression<Func<Product, bool>> GetExpressionForAdjacentPage(Product p)
  {
    throw new NotImplementedException();
  }
}