namespace Common;

public abstract partial record ServiceResult<T>(
  T? Value,
  bool IsSuccess,
  int StatusCode,
  string? ErrorMessage
) { }

public partial record ErrorServiceResult<T>(int StatusCode, string ErrorMessage)
  : ServiceResult<T>(default, false, StatusCode, ErrorMessage);

public partial record SuccessServiceResult<T>(T Value, int StatusCode)
  : ServiceResult<T>(Value, true, StatusCode, null);

public static partial class ServiceResults
{
  public static SuccessServiceResult<T> OkServiceResult<T>(T value) => new(value, 200);

  public static ErrorServiceResult<T> NotFoundServiceResult<T>(string errorMessage) => new(404, errorMessage);
}

public static partial class ServiceResultsExtensions
{
  public static IResult MapToIResult<T>(this ErrorServiceResult<T> errorResult)
  {
    return Results.Problem(errorResult.ErrorMessage, statusCode: errorResult.StatusCode);
  }
}
