using System.Linq.Expressions;

namespace ProductService.Utility.Filters;

public interface IFilterable<TItem>
{
  public object? FilterValue { get; set; }

  public IQueryable<TItem> ApplyFilterForOffsetPage(
    IQueryable<TItem> query);

  public Expression<Func<TItem, bool>> GetExpressionForAdjacentPage(
    TItem p);
}