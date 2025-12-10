using FluentResults;

namespace yeni.Domain.Common;

public class BaseResult
{
    public bool IsSuccess { get; }

    public Error.Error Error { get; }

    protected internal BaseResult(bool isSuccess, Error.Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsFailure => !IsSuccess;
    
    public static BaseResult Success() => new BaseResult(true, null);

    public static BaseResult Failure(Error.Error error) => new(false, error);
}