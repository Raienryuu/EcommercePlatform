using System.Linq.Expressions;
using ProductService.Models;
using EH = ProductService.Utility.EH<ProductService.Models.Product>;
using Exp =
  System.Linq.Expressions.Expression<
    System.Func<ProductService.Models.Product, bool>>;

namespace ProductService.Utility.OrderSetters;

public class PriceDescendingOrder : IOrderable<Product>
{
  public IQueryable<Product> ApplyOrderForNextPage(IQueryable<Product> query)
  {
    return query.OrderByDescending(q => q.Price)
      .ThenBy(q => q.Id);
  }

  public IQueryable<Product> ApplyOrderForPreviousPage(
    IQueryable<Product> query)
  {
    return query.OrderBy(q => q.Price)
      .ThenBy(q => q.Id);
  }

  public IQueryable<Product> ApplyOrderForOffsetPage(IQueryable<Product> query)
  {
    return ApplyOrderForNextPage(query);
  }

  public Exp IncludeReferencedItemForNextPage(Exp expression,
    Product refProduct)
  {
    expression = FilterItemsOutsideOfPriceRangeForNextPage(expression,
      refProduct);
    expression = AddItemsWithEqualPriceForNextPage(expression, refProduct);
    return expression;
  }

  public Exp IncludeReferencedItemForPreviousPage(Exp expression,
    Product refProduct)
  {
    expression = FilterItemsOutsideOfPriceRangeForPreviousPage(expression,
      refProduct);
    expression = AddItemsWithEqualPriceForPreviousPage(expression, refProduct);
    return expression;
  }

  private Exp FilterItemsOutsideOfPriceRangeForNextPage(Exp expression,
    Product refProduct)
  {
    return EH.CombineAsAnd(p => p.Price <
                               refProduct.Price, expression);
  }

  private Exp FilterItemsOutsideOfPriceRangeForPreviousPage(
    Exp expression, Product refProduct)
  {
    return EH.CombineAsAnd(p => p.Price >
                               refProduct.Price, expression);
  }

  private Exp AddItemsWithEqualPriceForNextPage(Exp expression,
    Product refProduct)
  {
    return EH.CombineAsOr(
      expression, q => q.Price == refProduct.Price && q.Id > refProduct.Id);
  }

  private static Exp AddItemsWithEqualPriceForPreviousPage(
    Exp expression,
    Product refProduct)
  {
    return EH.CombineAsOr(
      expression, q => q.Price == refProduct.Price && q.Id < refProduct.Id);
  }
}