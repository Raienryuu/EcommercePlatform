using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Utility.Filters;
using ProductService.Utility.OrderSetters;

namespace ProductService.Utility;

public class ProductsPagination(PaginationParams filters, ProductDbContext db)
{
  private readonly IOrderable<Product> _itemsOrderer = s_orderers[filters.Order];
  private readonly IEnumerable<IFilterable<Product>> _activeFilters = CreateFilters(filters);
  private readonly IQueryable<Product> _baseQuery = GetBaseQuery(filters, db);
  private static readonly Dictionary<PaginationParams.SortType, IOrderable<Product>> s_orderers = new()
  {
    { PaginationParams.SortType.PriceAsc, new PriceAscendingOrder() },
    { PaginationParams.SortType.PriceDesc, new PriceDescendingOrder() },
    { PaginationParams.SortType.QuantityAsc, new QuantityAscendingOrder() },
  };

  private static IQueryable<Product> GetBaseQuery(PaginationParams filters, ProductDbContext db)
  {
    if (filters.Category is not null)
      return db.GetProductsFromCategoryHierarchy((int)filters.Category).AsNoTracking();

    return db.Products.AsNoTracking();
  }

  private static List<IFilterable<Product>> CreateFilters(PaginationParams filters)
  {
    var filtersList = new List<IFilterable<Product>>();

    if (filters.MinPrice.HasValue)
      filtersList.Add(new MinPriceFilter(filters.MinPrice.Value));

    if (filters.MaxPrice.HasValue)
      filtersList.Add(new MaxPriceFilter(filters.MaxPrice.Value));

    if (filters.MinQuantity.HasValue)
      filtersList.Add(new MinQuantityFilter(filters.MinQuantity.Value));

    if (!string.IsNullOrEmpty(filters.Name))
      filtersList.Add(new NameFilter(filters.Name));

    return filtersList;
  }

  public IQueryable<Product> GetOffsetPageQuery(int pageNumber, int pageSize)
  {
    var filteredQuery = ExecuteFilters(_baseQuery);

    return _itemsOrderer
      .ApplyOrderForNextPage(filteredQuery)
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize);
  }

  public IQueryable<Product> GetNextPageQuery(int pageSize, Product product)
  {
    var filteredQuery = ExecuteFiltersForAdjacentPage(
      _baseQuery,
      product,
      _itemsOrderer.IncludeReferencedItemForNextPage
    );

    return _itemsOrderer.ApplyOrderForNextPage(filteredQuery).Take(pageSize);
  }

  public IQueryable<Product> GetPreviousPageQuery(int pageSize, Product product)
  {
    var filteredQuery = ExecuteFiltersForAdjacentPage(
      _baseQuery,
      product,
      _itemsOrderer.IncludeReferencedItemForPreviousPage
    );

    return _itemsOrderer.ApplyOrderForPreviousPage(filteredQuery).Take(pageSize).Reverse();
  }

  private IQueryable<Product> ExecuteFilters(IQueryable<Product> query)
  {
    return _activeFilters.Aggregate(query, (current, filter) => filter.ApplyFilterForOffsetPage(current));
  }

  private IQueryable<Product> ExecuteFiltersForAdjacentPage(
    IQueryable<Product> query,
    Product product,
    Func<Expression<Func<Product, bool>>, Product, Expression<Func<Product, bool>>> includeReferencedItem
  )
  {
    var combinedFilter = CombineFilters(product, includeReferencedItem);
    return query.Where(combinedFilter);
  }

  private Expression<Func<Product, bool>> CombineFilters(
    Product product,
    Func<Expression<Func<Product, bool>>, Product, Expression<Func<Product, bool>>> includeReferencedItem
  )
  {
    Expression<Func<Product, bool>> expression = e => true;
    expression = includeReferencedItem(expression, product);

    return _activeFilters.Aggregate(
      expression,
      (current, filter) => EH<Product>.CombineAsAnd(filter.GetExpressionForAdjacentPage(product), current)
    );
  }
}
