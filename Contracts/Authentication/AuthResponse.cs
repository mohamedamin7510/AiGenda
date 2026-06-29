namespace AI_genda_API.Contracts.Authentication;

public record AuthResponse
    (
    string Id,
    string FirstName, 
    string SecondName,
    string Email, 
    string Token , 
    int ExpiredIn,
    string RefreshToken,
    DateTime ExpiryDate
    );


