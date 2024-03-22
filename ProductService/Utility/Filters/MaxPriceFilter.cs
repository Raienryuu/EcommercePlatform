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

  public IQueryable<Product> ApplyConstraint(
    IQueryable<Product> query)
  {
    if (FilterValue is not null)
      query = query.Where(q => q.Price <= (decimal)FilterValue);
    return query;
  }

  public Exp GetExpressionForKeysetPagination(
    Product refProduct, bool isPreviousPage)
  {
    Exp expression = e => true;
    if (FilterValue is not null)
      expression = EH.CombineAsAnd(
        q => q.Price <= (decimal)FilterValue &&
             q.Id != refProduct.Id, expression);

    if (!IsMainFilter) return expression;

    expression =
      AddItemsWithEqualValue(expression, refProduct, isPreviousPage);

    expression =
      RemoveItemsOutsideOfValueRange(expression, refProduct, isPreviousPage);

    return expression;
  }

  private Exp RemoveItemsOutsideOfValueRange(
    Exp expression, Product refProduct,
    bool isPreviousPage)
  {
    if (!isPreviousPage)
      return EH.CombineAsOr(p => p.Price <
                                 refProduct.Price, expression);

    return EH.CombineAsOr(p => p.Price >
                               refProduct.Price, expression);
  }

  private static Exp AddItemsWithEqualValue(
    Exp expression,
    Product refProduct, bool isPreviousPage)
  {
    if (!isPreviousPage)
      return EH.CombineAsAnd(
        expression, q => q.Price == refProduct.Price && q.Id > refProduct.Id);

    return EH.CombineAsAnd(
      expression, q => q.Price == refProduct.Price && q.Id < refProduct.Id);
  }

  public IQueryable<Product> ApplyOrder(IQueryable<Product> query,
    bool isPreviousPage = false)
  {
    if (!IsMainFilter) return query;

    return isPreviousPage
      ? query.OrderBy(q => q.Price)
        .ThenBy(q => q.Id)
      : query.OrderByDescending(q => q.Price)
        .ThenBy(q => q.Id);
  }
}