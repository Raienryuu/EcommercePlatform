using System.Linq.Expressions;
using ProductService.Models;

namespace ProductService.Utility.OrderSetters;

public interface IOrderable<TItem>
{
  public IQueryable<TItem> ApplyOrderForNextPage(IQueryable<TItem> query);
  public IQueryable<TItem> ApplyOrderForPreviousPage(IQueryable<TItem> query);
  public IQueryable<TItem> ApplyOrderForOffsetPage(IQueryable<TItem> query);

  public Expression<Func<TItem, bool>> IncludeReferencedItemForNextPage(
    Expression<Func<TItem, bool>> expression,
    Product refProduct);

  public Expression<Func<TItem, bool>> IncludeReferencedItemForPreviousPage(
    Expression<Func<TItem, bool>> expression,
    Product refProduct);
}