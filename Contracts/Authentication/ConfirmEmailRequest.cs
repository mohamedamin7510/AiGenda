namespace BucketSurvey.Api.Contract.Authentication;

public record ConfirmEmailRequest
(
    string userId , 
    string Code     
);
