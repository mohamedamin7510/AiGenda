using AI_genda_API.Abstractions.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

public class ExtendedUser:IdentityUser
{
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public int SubscriptionType { get; set; } = (int)UserType.Free;
    public string? JobTitle { get; set; }
    public string? AvatarUrl { get; set; }
    public DateOnly? DateOfBirth { get; set; } = default;
    public bool IsDisabled { get; set; } = false;

    public List<WorkspaceMember> WorkSpaceMembers { get; set; } = [];
    public List<RefreshToken> RefreshTokens { get; set; } = []; 

 
}
