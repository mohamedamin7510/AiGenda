namespace AI_genda_API.Errors;

public static  class UserErrors
{
    public static Error EmailnotFounded = new Error("Email.NotFounded" , "This Email is not registerd" , StatusCodes.Status404NotFound);
        
    public static Error EmailNotConfirmed = new Error("Email.NotConfirmed ", "This Email is not confirmed", StatusCodes.Status401Unauthorized);
    
    public static readonly Error Invalidcredentails = new( "User.Invalidcredentails", "Sorry can't register because email or password is wrong Or both", StatusCodes.Status401Unauthorized);       
   
    public static readonly Error UserLockedOut = new( "User.LockedOut", "You tried more than 5 times , So you blocked now!", StatusCodes.Status423Locked);       
          
    public static readonly Error InvalidToken = new( "User.InvalidToken", "Invalid Token!", StatusCodes.Status401Unauthorized);
      
    public static readonly Error EmailDuplicated = new( "User.DuplicatedEmail", "Email is registered already can't be added!", StatusCodes.Status409Conflict);   
            
    public static readonly Error SubscriptionModel =  new( "User.SubscriptionModel", "You should subscribe on the premium model !", StatusCodes.Status409Conflict);  
                  
    public static readonly Error InvalidCode = new( "User.InvaildCode", "The provided code is not vaild !", StatusCodes.Status400BadRequest);
            
    public static Error ActiveConfirmedEmail = new Error("User.ActiveConfirmedEmail",  "This email is actually confirmed!", StatusCodes.Status400BadRequest);
      
    public static Error UserIsDeleted = new Error("User.AccountDeleted",  "This user is actually deleted", StatusCodes.Status404NotFound); 
    public static Error AddedMemberBefore  =>   new Error("User.ActuallyAdded", "This member is added to this workspace before", StatusCodes.Status400BadRequest);
    public static Error UserNotFounded  =>   new Error("User.NotFounded", "you are not founded in AiGenda!", StatusCodes.Status401Unauthorized);
}
