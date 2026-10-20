namespace BBLiftClient.Application.Common;

public class Result<T>
{
    public Result(bool isSuccess, T? data, string errorMessage)
    {
        this.IsSuccess = isSuccess;
        this.Data = data;
        this.ErrorMessage = errorMessage;
    }
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string ErrorMessage { get; }
    public static Result<T> Success(T data) => new(true, data, string.Empty);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
}

public class Result
{
    public Result(bool isSuccess, string errorMessage)
    {
        this.IsSuccess = isSuccess;
        this.ErrorMessage = errorMessage;
    }
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string errorMessage) => new(false, errorMessage);
}