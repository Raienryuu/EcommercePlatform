namespace Common;

public abstract partial record ServiceResult<T>(
  T? Value,
  bool IsSuccess,
  int StatusCode,
  string? ErrorMessage
);

public partial record ErrorServiceResult<T>(int StatusCode, string ErrorMessage)
  : ServiceResult<T>(default, false, StatusCode, ErrorMessage);

public partial record SuccessServiceResult<T>(T Value, int StatusCode)
  : ServiceResult<T>(Value, true, StatusCode, null);

public static partial class ServiceResults
{
  public static SuccessServiceResult<T> Success<T>(T value, int statusCode) => new(value, statusCode);

  public static SuccessServiceResult<T> Ok<T>(T value) => new(value, 200);

  public static ErrorServiceResult<T> Error<T>(string errorMessage, int statusCode) =>
    new(statusCode, errorMessage);

  public static ErrorServiceResult<T> NotFound<T>(string errorMessage) =>
    new(404, errorMessage);
}
