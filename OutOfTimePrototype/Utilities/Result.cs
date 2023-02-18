namespace OutOfTimePrototype.Utilities;

public class Result
{
    protected Result()
    {
        State = ResultState.Succeed;
        Error = default;
    }

    protected Result(Exception error)
    {
        State = ResultState.Failed;
        Error = error;
    }

    public Exception? Error { get; }
    protected ResultState State { get; }

    public bool IsSucceed => State == ResultState.Succeed;
    public bool IsFailed => State == ResultState.Failed;

    public static Result Fail(Exception e)
    {
        return new Result(e);
    }

    public static Result<T> Fail<T>(Exception e)
    {
        return new Result<T>(e);
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    public void Match(Action success, Action<Exception> fail)
    {
        if (IsSucceed) success();
        else fail(Error);
    }

    public TResult Match<TResult>(Func<TResult> success, Func<Exception, TResult> fail)
    {
        return IsSucceed ? success() : fail(Error);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        return obj is Result other && State == other.State && Error == other.Error;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Error, (int)State);
    }

    protected enum ResultState
    {
        Succeed,
        Failed
    }
}

public class Result<T> : Result
{
    internal Result(T value)
    {
        Value = value;
    }

    internal Result(Exception e) : base(e)
    {
        Value = default;
    }

    public T? Value { get; }

    public void Match(Action<T> succeed, Action<Exception> fail)
    {
        if (IsSucceed) succeed(Value);
        else fail(Error);
    }

    public TResult Match<TResult>(Func<T, TResult> succeed, Func<Exception, TResult> fail)
    {
        return IsSucceed ? succeed(Value) : fail(Error);
    }
}