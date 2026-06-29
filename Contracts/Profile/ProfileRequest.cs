namespace BucketSurvey.Api.Contract.User;

public record ProfileRequest
(
    string FirstName,
        string SecondName ,
          string JobTitle, 
             DateOnly DateOfBirth 
);

