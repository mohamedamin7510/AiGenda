namespace AI_genda_API.Contracts.Authentication;

public record RegisterRequest
(
    string FirstName,
    string LastName,
    string Email , 
    string Password,
    string ConfirmPassword
    
);


