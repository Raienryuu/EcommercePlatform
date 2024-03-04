using System.Linq.Expressions;
using ProductService.Models;

namespace ProductService.Utility;

public class ProductsPagination
{
  private SearchFilters _filters;
  private IQueryable<Product> _query;

  public ProductsPagination(SearchFilters filters,
    ProductDbContext db)
  {
    _filters = filters;
    _query = db.Products.AsQueryable();
  }

  public IQueryable<Product> GetOffsetPageQuery(int pageNumber, int pageSize)
  {
    var filterExpression = ApplySearchFiltersForOffset();
    _query = _query.Where(filterExpression);
    _query = ApplySortType();
    return _query.Skip((pageNumber - 1) * pageSize)
      .Take(pageSize);
  }

  public IQueryable<Product> GetAdjacentPageQuery(int pageSize,
    bool isPreviousPage, Product product)
  {
    var filterExpression = ApplySearchFiltersForKeyset(product);
    var keysetExpression =
      AddProductsWithEqualSortingValue(product, isPreviousPage);

    var combinedExpression = ExpressionHelper<Product>.CombineAsOr(
      keysetExpression, filterExpression);
    _query = _query.Where(combinedExpression);
    ApplySortType();
    return _query.Take(pageSize);
  }

  private Expression<Func<Product, bool>> AddProductsWithEqualSortingValue(
    Product product, bool isPreviousPage)
  {
    Expression<Func<Product, bool>>? keysetExpression;
    switch (_filters.Order)
    {
      case SearchFilters.SortType.MinPrice:
      case SearchFilters.SortType.MaxPrice:
        keysetExpression = AddProductsWithEqualPrice(product, isPreviousPage);
        break;
      default:
        throw new InvalidDataException("SortType in SearchFilter" +
                                       " is invalid");
    }
    return keysetExpression;
  }

  private Expression<Func<Product, bool>> AddProductsWithEqualPrice(
    Product product, bool isPreviousPage)
  {
    if (isPreviousPage)
    {
      return p => p.Price == product.Price && p.Id < product.Id;
    }

    return p => p.Price == product.Price && p.Id > product.Id;
  }


  private IQueryable<Product> ApplySortType()
  {
    _query = _filters.Order switch
    {
      SearchFilters.SortType.MinPrice => _query.OrderBy(p => p.Price),
      SearchFilters.SortType.MaxPrice => _query.OrderByDescending(p => p.Price),
      _ => throw new InvalidDataException("SortType in SearchFilter is invalid")
    };

    _query = (_query as IOrderedQueryable<Product>)!.ThenBy(p => p.Id);

    return _query;
  }

  private Expression<Func<Product, bool>> ApplySearchFiltersForOffset()
  {
    Expression<Func<Product, bool>> filterExpr = product => true;

    if (_filters.Name is not null)
    {
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Name.Contains(_filters.Name,
          StringComparison.InvariantCultureIgnoreCase), filterExpr);
    }
    if (_filters.MinPrice is not null)
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Price >= _filters.MinPrice, filterExpr);
    if (_filters.MaxPrice is not null)
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Price <= _filters.MaxPrice, filterExpr);
    if (_filters.MinQuantity is not null)
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Quantity >= _filters.MinQuantity, filterExpr);

    return filterExpr;
  }

  private Expression<Func<Product, bool>> ApplySearchFiltersForKeyset(
    Product product)
  {
    Expression<Func<Product, bool>> filterExpr = p => true;

    if (_filters.Name is not null)
    {
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Name.Contains(_filters.Name), filterExpr);
    }
    if (_filters.MinPrice is not null)
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Price >= _filters.MinPrice && p.Id != product.Id,
        filterExpr);
    if (_filters.MaxPrice is not null)
    {
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Price <= _filters.MaxPrice && p.Id != product.Id,
        filterExpr);
    }
    if (_filters.MinQuantity is not null)
      filterExpr = ExpressionHelper<Product>.CombineAsAnd(
        p => p.Quantity >= _filters.MinQuantity && p.Id != product.Id,
        filterExpr);

    return filterExpr;
  }
}