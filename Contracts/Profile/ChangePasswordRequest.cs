namespace BucketSurvey.Api.Contract.User;

public record ChangePasswordRequest
(
    string CurrentPassword, 
            string NewPassword  
);
