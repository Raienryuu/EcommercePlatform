using System.Diagnostics;
using ProductService.Models;
using EH = ProductService.Utility.EH<ProductService.Models.Product>;
using Exp =
  System.Linq.Expressions.Expression<
    System.Func<ProductService.Models.Product, bool>>;

namespace ProductService.Utility.Filters;

public class MaxPriceFilter(
  SearchFilters.SortType order,
  object? filterValue = null)
  : IFilterable<Product>
{
  public bool IsMainFilter { get; set; } =
    order == SearchFilters.SortType.PriceDesc;

  public object? FilterValue { get; set; } = filterValue;

  public Exp GetExpressionForAdjacentPage(Product refProduct)
  {
    Exp expression = e => true;
    if (FilterValue is not null)
      expression = EH.CombineAsAnd(
        q => q.Price <= (decimal)FilterValue, expression);
    return expression;
  }


  public IQueryable<Product> ApplyFilterForOffsetPage(
    IQueryable<Product> query)
  {
    if (FilterValue is not null)
      query = query.Where(q => q.Price <= (decimal)FilterValue);
    return query;
  }
}