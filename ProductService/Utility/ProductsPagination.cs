using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility.Filters;
using ProductService.Utility.OrderSetters;
using Exp = System.Linq.Expressions.Expression<System.Func<ProductService.Models.Product, bool>>;

namespace ProductService.Utility;

public class ProductsPagination
{
  private readonly Dictionary<PaginationParams.SortType, IOrderable<Product>> _orderers = new()
  {
    { PaginationParams.SortType.PriceAsc, new PriceAscendingOrder() },
    { PaginationParams.SortType.PriceDesc, new PriceDescendingOrder() },
    { PaginationParams.SortType.QuantityAsc, new QuantityAscendingOrder() },
  };

  private IQueryable<Product> _query;
  private readonly IEnumerable<IFilterable<Product>> _activeFilters;
  private readonly IOrderable<Product> _itemsOrderer;

  public ProductsPagination(PaginationParams filters, ProductDbContext db)
  {
    _query = db.Products.AsNoTracking();
    _activeFilters = CreateFilters(filters);
    _itemsOrderer = _orderers[filters.Order];
  }

  public IQueryable<Product> GetOffsetPageQuery(int pageNumber, int pageSize)
  {
    ExecuteFilters();

    _query = _itemsOrderer.ApplyOrderForNextPage(_query);

    return _query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
  }

  private void ExecuteFilters()
  {
    foreach (var f in _activeFilters)
    {
      _query = f.ApplyFilterForOffsetPage(_query);
    }
  }

  private static List<IFilterable<Product>> CreateFilters(PaginationParams filters)
  {
    var list = new List<IFilterable<Product>>
    {
      new MinPriceFilter(filters.MinPrice),
      new MaxPriceFilter(filters.MaxPrice),
      new MinQuantityFilter(filters.MinQuantity),
      new NameFilter(filters.Name),
    };
    return list;
  }

  public IQueryable<Product> GetNextPageQuery(int pageSize, Product product)
  {
    ExecuteFiltersForNextPage(product);
    return _query.Take(pageSize);
  }

  public IQueryable<Product> GetPreviousPageQuery(int pageSize, Product product)
  {
    ExecuteFiltersForPreviousPage(product);
    return _query.Take(pageSize).Reverse();
  }

  private void ExecuteFiltersForNextPage(Product p)
  {
    var expression = CombineFilters(p);
    expression = _itemsOrderer.IncludeReferencedItemForNextPage(expression, p);
    _query = _query.Where(expression);
    _query = _itemsOrderer.ApplyOrderForNextPage(_query);
  }

  private void ExecuteFiltersForPreviousPage(Product p)
  {
    var expression = CombineFilters(p);
    expression = _itemsOrderer.IncludeReferencedItemForPreviousPage(expression, p);
    _query = _query.Where(expression);
    _query = _itemsOrderer.ApplyOrderForPreviousPage(_query);
  }

  private Exp CombineFilters(Product p)
  {
    Exp expression = e => true;
    return _activeFilters.Aggregate(
      expression,
      (current, filter) => EH<Product>.CombineAsAnd(filter.GetExpressionForAdjacentPage(p), current)
    );
  }
}
