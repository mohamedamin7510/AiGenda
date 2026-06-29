namespace AI_genda_API.Contracts.Profile;

public record ConfirmChangeEmailRequest
(
    string Id , 
    string newemail,
    string Code
);
