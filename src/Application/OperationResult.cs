namespace Application;

public class OperationResult<T>
{
    public bool Success { get; private init; }
    public string? ErrorMessage { get; private init; }
    public T? Data { get; private init; }

    public static OperationResult<T> SuccessResult(T data)
    {
        return new OperationResult<T>
        {
            Success = true, Data = data
        };
    }

    public static OperationResult<T> ErrorResult(string errorMessage)
    {
        return new OperationResult<T>
        {
            Success = false, ErrorMessage = errorMessage
        };
    }
}