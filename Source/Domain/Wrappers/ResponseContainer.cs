namespace Domain.Wrappers;

public class ResponseContainer<T>
{
    public T Response { get; set; }
    public bool IsSucceed { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }

    public ResponseContainer(T response)
    {
        Response = response;
    }

    public ResponseContainer(T response, bool isSucceed)
    {
        Response = response;
        IsSucceed = isSucceed;
    }

    public ResponseContainer(bool isSucceed, string errorCode, string errorMessage)
    {
        IsSucceed = isSucceed;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public ResponseContainer(T response, bool isSucceed, string errorCode, string errorMessage)
    {
        Response = response;
        IsSucceed = isSucceed;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
