using System.Net;

namespace Common;

public abstract partial record ServiceResult<T>
{
  private readonly T _value;
  public readonly bool IsSuccess;
  public bool IsFailure => !IsSuccess;
  public readonly HttpStatusCode StatusCode;
  public readonly string? ErrorMessage;

  protected ServiceResult(T value, bool isSuccess, HttpStatusCode statusCode, string? errorMessage)
  {
    _value = value;
    IsSuccess = isSuccess;
    StatusCode = statusCode;
    ErrorMessage = errorMessage;
  }

  public T Value =>
    IsSuccess
      ? _value
      : throw new InvalidOperationException("Trying to access Value when result is Failure.");
}

public abstract partial record ServiceResult(bool IsSuccess, HttpStatusCode StatusCode, string? ErrorMessage)
{
  public bool IsFailure => !IsSuccess;
};

//Non-generic

public partial record ErrorServiceResult(HttpStatusCode StatusCode, string ErrorMessage)
  : ServiceResult(false, StatusCode, ErrorMessage);

public partial record SuccessServiceResult(HttpStatusCode StatusCode) : ServiceResult(true, StatusCode, null);

public partial record ErrorServiceResult<T>(HttpStatusCode StatusCode, string ErrorMessage)
  : ServiceResult<T>(default!, false, StatusCode, ErrorMessage);

public partial record SuccessServiceResult<T>(T Value, HttpStatusCode StatusCode)
  : ServiceResult<T>(Value, true, StatusCode, null);

public static partial class ServiceResults
{
  public static SuccessServiceResult<T> Success<T>(T value, HttpStatusCode statusCode) => new(value, statusCode);

  public static SuccessServiceResult<T> Ok<T>(T value) => new(value, HttpStatusCode.OK);

  public static ErrorServiceResult<T> Error<T>(string errorMessage, HttpStatusCode statusCode) =>
    new(statusCode, errorMessage);

  public static ErrorServiceResult<T> NotFound<T>(string errorMessage) => new(HttpStatusCode.NotFound, errorMessage);

  //Non-generic

  public static SuccessServiceResult Success(HttpStatusCode statusCode) => new(statusCode);

  public static ErrorServiceResult Error(string errorMessage, HttpStatusCode statusCode) =>
    new(statusCode, errorMessage);

  //Changing type of ErrorServiceResult
  public static ErrorServiceResult<K> RemapError<T, K>(this ServiceResult<T> result) =>
    new(result.StatusCode, result.ErrorMessage!);

  public static ErrorServiceResult RemapError<T>(this ServiceResult<T> result) =>
    new(result.StatusCode, result.ErrorMessage!);
}
