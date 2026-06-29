namespace AI_genda_API.Contracts.ProfileSetting;

public record ProfileResponse
(
   string Id,
   string FirstName,
   string SecondName,
   string Email,
   DateOnly DateOfBirth,
   string JobTitle,
   string AvatarUrl
);

