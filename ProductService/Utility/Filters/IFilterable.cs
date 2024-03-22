using System.Linq.Expressions;
using ProductService.Models;

namespace ProductService.Utility.Filters;

public interface IFilterable<TItem>
{
  public object? FilterValue { get; set; }
  public bool IsMainFilter { get; set; }

  public IQueryable<TItem> ApplyConstraint(
    IQueryable<TItem> query);

  public Expression<Func<TItem, bool>> GetExpressionForKeysetPagination(
    TItem p, bool isPreviousPage);

  public IQueryable<TItem> ApplyOrder(IQueryable<TItem> query,
    bool isPreviousPage = false);
}