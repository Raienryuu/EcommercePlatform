using ProductService.Models;
using EH = ProductService.Utility.EH<ProductService.Models.Product>;
using Exp =
  System.Linq.Expressions.Expression<
    System.Func<ProductService.Models.Product, bool>>;

namespace ProductService.Utility.OrderSetters;

public class QuantityAscendingOrder : IOrderable<Product>
{
  public IQueryable<Product> ApplyOrderForNextPage(IQueryable<Product> query)
  {
    return query.OrderBy(q => q.Quantity)
      .ThenBy(q => q.Id);
  }

  public IQueryable<Product> ApplyOrderForPreviousPage(
    IQueryable<Product> query)
  {
    return query.OrderByDescending(q => q.Quantity)
      .ThenBy(q => q.Id);
  }

  public IQueryable<Product> ApplyOrderForOffsetPage(IQueryable<Product> query)
  {
    return ApplyOrderForNextPage(query);
  }

  public Exp IncludeReferencedItemForNextPage(Exp expression,
    Product refProduct)
  {
    expression = FilterItemsOutsideOfQuantityRangeForNextPage(expression,
      refProduct);
    expression = AddItemsWithEqualQuantityForNextPage(expression, refProduct);
    return expression;
  }

  public Exp IncludeReferencedItemForPreviousPage(Exp expression,
    Product refProduct)
  {
    expression = FilterItemsOutsideOfQuantityRangeForPreviousPage(expression,
      refProduct);
    expression = AddItemsWithEqualQuantityForPreviousPage(expression, refProduct);
    return expression;
  }

  private Exp FilterItemsOutsideOfQuantityRangeForNextPage(Exp expression,
    Product refProduct)
  {
    return EH.CombineAsAnd(p => p.Quantity >
                                refProduct.Quantity, expression);
  }

  private Exp FilterItemsOutsideOfQuantityRangeForPreviousPage(
    Exp expression, Product refProduct)
  {
    return EH.CombineAsAnd(p => p.Quantity <
                                refProduct.Quantity, expression);
  }

  private Exp AddItemsWithEqualQuantityForNextPage(Exp expression,
    Product refProduct)
  {
    return EH.CombineAsOr(
      expression,
      q => q.Quantity == refProduct.Quantity && q.Id > refProduct.Id);
  }

  private static Exp AddItemsWithEqualQuantityForPreviousPage(
    Exp expression,
    Product refProduct)
  {
    return EH.CombineAsOr(
      expression,
      q => q.Quantity == refProduct.Quantity && q.Id < refProduct.Id);
  }
}