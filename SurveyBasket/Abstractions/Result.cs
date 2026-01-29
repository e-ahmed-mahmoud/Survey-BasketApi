namespace SurveyBasket.Abstractions;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error { get; } = default!;

    public Result(bool isSuccess, Error error)
    {
        // throw if the state is invalid = success with Error code
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException("Invalid result state.");

        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    //define new method for generic result
    public static Result<TValue> Success<TValue>(TValue value) => new(isSuccess: true, Error.None, value);
    public static Result<TValue> Failure<TValue>(Error error) => new(isSuccess: false, error, default!);
}

// define Result <T> to hold value in case of success
public class Result<TValue> : Result
{
    private readonly TValue _value = default!;
    public Result(bool isSuccess, Error error, TValue value) : base(isSuccess, error)
    {
        this._value = value;
    }
    public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access the value of a failed result.");
}
