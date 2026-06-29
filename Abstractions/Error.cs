namespace AI_genda_API.Abstractions;

public record Error
{
    public string Code { get; }
    public string Descrption { get; }
    public int?  StatusCode { get; }

    public Error(string Code, string Message ,int statusCode)
    {
        this.Code = Code;
        this.Descrption = Message;
        this.StatusCode = statusCode;
    }

    public static Error None =>  new Error(string.Empty, string.Empty , default);


}
