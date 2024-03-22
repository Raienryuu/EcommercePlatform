using System.Linq.Expressions;

namespace ProductService.Utility;

public static class EH<T>
{
  public static Expression<Func<T, bool>> CombineAsAnd(
    Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
  {
    var invokedExpression =
      Expression.Invoke(exp1, exp2.Parameters);

    var combinedExpressionBody =
      Expression.AndAlso(exp2.Body, invokedExpression);

    var combinedExpression = Expression
      .Lambda<Func<T, bool>>(combinedExpressionBody,
        exp2.Parameters);
    return combinedExpression;
  }

  public static Expression<Func<T, bool>> CombineAsOr(
    Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
  {
    var invokedExpression =
      Expression.Invoke(exp1, exp2.Parameters);

    var combinedExpressionBody =
      Expression.Or(exp2.Body, invokedExpression);

    var combinedExpression = Expression
      .Lambda<Func<T, bool>>(combinedExpressionBody,
        exp2.Parameters);
    return combinedExpression;
  }
}