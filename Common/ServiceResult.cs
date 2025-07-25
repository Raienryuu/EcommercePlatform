namespace Common;

public abstract partial record ServiceResult<T>
{
  private readonly T _value;
  public readonly bool IsSuccess;
  public readonly int StatusCode;
  public readonly string? ErrorMessage;

  protected ServiceResult(T value, bool isSuccess, int statusCode, string? errorMessage)
  {
    _value = value;
    IsSuccess = isSuccess;
    StatusCode = statusCode;
    ErrorMessage = errorMessage;
  }

  public T Value =>
    IsSuccess
      ? _value
      : throw new InvalidOperationException("Trying to access Value when IsSuccess is false.");
}

public abstract partial record ServiceResult(bool IsSuccess, int StatusCode, string? ErrorMessage);

//Non-generic

public partial record ErrorServiceResult(int StatusCode, string ErrorMessage)
  : ServiceResult(false, StatusCode, ErrorMessage);

public partial record SuccessServiceResult(int StatusCode) : ServiceResult(true, StatusCode, null);

public partial record ErrorServiceResult<T>(int StatusCode, string ErrorMessage)
  : ServiceResult<T>(default!, false, StatusCode, ErrorMessage);

public partial record SuccessServiceResult<T>(T Value, int StatusCode)
  : ServiceResult<T>(Value, true, StatusCode, null);

public static partial class ServiceResults
{
  public static SuccessServiceResult<T> Success<T>(T value, int statusCode) => new(value, statusCode);

  public static SuccessServiceResult<T> Ok<T>(T value) => new(value, 200);

  public static ErrorServiceResult<T> Error<T>(string errorMessage, int statusCode) =>
    new(statusCode, errorMessage);

  public static ErrorServiceResult<T> NotFound<T>(string errorMessage) => new(404, errorMessage);

  //Non-generic

  public static SuccessServiceResult Success(int statusCode) => new(statusCode);

  public static ErrorServiceResult Error(string errorMessage, int statusCode) =>
    new(statusCode, errorMessage);
}
