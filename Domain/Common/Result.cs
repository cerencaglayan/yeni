namespace yeni.Domain.Common;


public class Result<TValue> : BaseResult
{
    public TValue? Value { get; }

    private Result(bool isSuccess, Error.Error? error, TValue? value)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<TValue> Success(TValue value)
        => new(true, null, value);

    public static Result<TValue> Failure(Error.Error error)
        => new(false, error, default);

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null
            ? Success(value)
            : Failure(null);
}
