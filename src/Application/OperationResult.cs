namespace Application;

public class OperationResult<T>
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public T Data { get; set; }

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