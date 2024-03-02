namespace TPSBackend.Dtos;

public class ResponseMessage
{
    public ResponseMessage(string title, string message)
    {
        Title = title;
        Message = message;
    }

    public string Title { get; set; }
    public string Message { get; set; }
}