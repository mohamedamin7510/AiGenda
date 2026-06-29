namespace AI_genda_API.Entities;

[Owned]
public class RefreshToken
{
    public string refreshToken { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public DateTime ExpiredAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiredAt;
    public bool IsActive => RevokedAt is null && !IsExpired;



}
