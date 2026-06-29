namespace BucketSurvey.Api.Contract.Authentication;

public record ResetPasswordRequest
(
    string email , 
    string code , 
    string newpassword
);
