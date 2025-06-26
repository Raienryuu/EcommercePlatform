using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility.Filters;
using ProductService.Utility.OrderSetters;
using Exp = System.Linq.Expressions.Expression<System.Func<ProductService.Models.Product, bool>>;

namespace ProductService.Utility;

public class ProductsPagination(PaginationParams filters, ProductDbContext db)
{
  private IQueryable<Product> _query = GetBaseQuery(filters, db);
  private readonly IOrderable<Product> _itemsOrderer = s_orderers[filters.Order];
  private readonly IEnumerable<IFilterable<Product>> _activeFilters = CreateFilters(filters);

  private static IQueryable<Product> GetBaseQuery(PaginationParams filters, ProductDbContext db)
  {
    if (filters.Category is not null)
    {
      return db.GetProductsFromCategoryHierarchy((int)filters.Category).AsNoTracking();
    }

    return db.Products.AsNoTracking();
  }

  private static readonly Dictionary<PaginationParams.SortType, IOrderable<Product>> s_orderers = new()
  {
    { PaginationParams.SortType.PriceAsc, new PriceAscendingOrder() },
    { PaginationParams.SortType.PriceDesc, new PriceDescendingOrder() },
    { PaginationParams.SortType.QuantityAsc, new QuantityAscendingOrder() },
  };

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
