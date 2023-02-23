namespace OutOfTimePrototype.Utilities;

/// <summary>
///     Class for returning a result without throwing exception, when exception is expected and common,
///     e.g. invalid data passed by the user
/// </summary>
/// <example>
///     <code>
/// public void Save(ClassDto classDto); // The method is Action and can't fail
/// public List&#60;Class&#62; GetAll(); // The method is Function and can't fail
/// public Result Create(Guid id); // The method is Action and can fail
/// public Result&#60;Class&#62; GetBy(Guid id); // The method is Function and can fail
/// </code>
/// </example>
public class Result
{
    protected Result()
    {
        State = ResultState.Succeed;
        Error = null;
    }

    protected Result(Exception error)
    {
        State = ResultState.Failed;
        Error = error;
    }

    public Exception? Error { get; }
    protected ResultState State { get; }

    public bool IsSucceed => State == ResultState.Succeed && Error == null;
    public bool IsFailed => State == ResultState.Failed && Error != null;

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

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Exception exception)
    {
        return new Result<T>(exception);
    }

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