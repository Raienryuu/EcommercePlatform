using System.Diagnostics;
using ProductService.Models;
using EH = ProductService.Utility.EH<ProductService.Models.Product>;
using Exp =
  System.Linq.Expressions.Expression<
    System.Func<ProductService.Models.Product, bool>>;

namespace ProductService.Utility.Filters;

public class NameFilter(
  SearchFilters.SortType order,
  object? filterValue = null)
  : IFilterable<Product>
{
  public bool IsMainFilter { get; set; } = false;

  public object? FilterValue { get; set; } = filterValue;

  public IQueryable<Product> ApplyConstraint(
    IQueryable<Product> query)
  {
    if (FilterValue is not null)
      query = query.Where(q => q.Name.Contains((string)FilterValue));
    return query;
  }

  public Exp GetExpressionForKeysetPagination(
    Product refProduct, bool isPreviousPage)
  {
    Exp expression = e => true;

    if (FilterValue is not null)
      expression = EH.CombineAsAnd(
        q => q.Name.Contains((string)FilterValue) &&
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
      return EH.CombineAsAnd(p => p.Quantity >=
                                  refProduct.Quantity, expression);

    return EH.CombineAsAnd(p => p.Quantity <=
                                refProduct.Quantity, expression);
  }

  public IQueryable<Product> ApplyOrder(IQueryable<Product> query,
    bool isPreviousPage)
  {
    return IsMainFilter
      ? query
        .OrderBy(q => q.Quantity)
        .ThenBy(q => q.Id)
      : query;
  }

  private static Exp AddItemsWithEqualValue(
    Exp e,
    Product refProduct, bool isPreviousPage)
  {
    if (!isPreviousPage)
      return EH.CombineAsOr(
        e, q => q.Quantity == refProduct.Quantity && q.Id > refProduct.Id);

    return EH.CombineAsOr(
      e, q => q.Quantity == refProduct.Quantity && q.Id < refProduct.Id);
  }
}