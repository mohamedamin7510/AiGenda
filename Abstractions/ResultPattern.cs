namespace AI_genda_API.Abstractions;

public class Result
{
    public bool IsSuccess { get; set; } = true;
    public bool IsFaluire  => !IsSuccess;
    public Error Error { get; set; }

    public Result(bool IsSuccess , Error error)
    {

        if (IsSuccess && error != Error.None || IsFaluire && error == Error.None )
            throw new InvalidOperationException("There is upnormal thing in the error property");

        this.IsSuccess = IsSuccess;
        this.Error = error!;
    }


    public static Result  Success() => new Result(true , Error.None!);
    public static Result  Faluire(Error error) => new Result(false , error);
    public static Result<Type> Success<Type>(Type value) => new Result<Type>(value, true , Error.None!);
    public static Result<Type> Faluire<Type>(Error error) => new Result<Type>(default!,false , error);


}


public class Result<Type> : Result
{

    private readonly Type _Value; 
    public Result(Type Value,bool IsSucces , Error error ) : base(IsSucces , error)
    {
        _Value = Value!;
    }

    public Type Value => IsSuccess ? _Value : throw new InvalidOperationException("Faluire results can't hvae value");

}