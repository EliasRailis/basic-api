namespace HaefeleSoftware.Api.Domain.Types;

public readonly struct Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;
    
    public bool IsError { get; }
    
    public bool IsSuccess => !IsError;

    public Result(TError error)
    {
        IsError = true;
        _error = error;
        _value = default;
    }
    
    public Result(TValue value)
    {
        IsError = false;
        _value = value;
        _error = default;
    }
    
    public static implicit operator Result<TValue, TError>(TValue value) => new(value);
    
    public static implicit operator Result<TValue, TError>(TError error) => new(error);
    
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onError)
    {
        return !IsError ? onSuccess(_value!) : onError(_error!);
    }
}