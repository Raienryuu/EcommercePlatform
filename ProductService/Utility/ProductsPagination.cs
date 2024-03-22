using ProductService.Models;
using ProductService.Utility.Filters;
using Exp = System.Linq.Expressions.Expression<System.Func<
  ProductService.Models.Product, bool>>;


namespace ProductService.Utility;

public class ProductsPagination(
  SearchFilters filters,
  ProductDbContext db)
{
  private IQueryable<Product> _query = db.Products.AsQueryable();

  public IQueryable<Product> GetOffsetPageQuery(
    int pageNumber, int pageSize)
  {
    var filtersL = CreateFilters();
    ExecuteFilters(filtersL);

    return _query.Skip((pageNumber - 1) * pageSize)
      .Take(pageSize);
  }

  private void ExecuteFilters(List<IFilterable<Product>> filtersL)
  {
    foreach (var f in filtersL)
    {
      _query = f.ApplyConstraint(_query);
      _query = f.ApplyOrder(_query);
    }
  }


  private List<IFilterable<Product>> CreateFilters()
  {
    var list = new List<IFilterable<Product>>
    {
      new MinPriceFilter(
        filters.Order,
        filters.MinPrice),
      new MaxPriceFilter(
        filters.Order,
        filters.MaxPrice),
      new MinQuantityFilter(
        filters.Order,
        filters.MinQuantity),
      new NameFilter(
        filters.Order,
        filters.Name)
    };
    return list;
  }


  public IQueryable<Product> GetAdjacentPageQuery(int pageSize,
    bool isPreviousPage, Product product)
  {
    var filtersL = CreateFilters();
    ExecuteFilters(filtersL, product, isPreviousPage);

    return isPreviousPage
      ? _query.Take(pageSize).Reverse()
      : _query.Take(pageSize);
  }

  private void ExecuteFilters(List<IFilterable<Product>> filtersL,
    Product p, bool isPreviousPage)
  {
    var expression = ChainFilters(filtersL,
      p, isPreviousPage);
    _query = _query.Where(expression);
    foreach (var filter in filtersL)
      _query = filter.ApplyOrder(_query, isPreviousPage);
  }

  private static Exp ChainFilters(List<IFilterable<Product>> filtersL,
    Product p, bool isPreviousPage)
  {
    Exp expression = e => true;
    return filtersL.Aggregate(expression,
      (current, filter) =>
        EH<Product>.CombineAsAnd(
          filter.GetExpressionForKeysetPagination(
            p, isPreviousPage), current));
  }
}