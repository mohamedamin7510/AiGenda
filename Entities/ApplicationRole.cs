namespace AI_genda_API.Entities;

public class ApplicationRole : IdentityRole<string>
{
    public bool IsDeleted { get; set; } = false;
    public bool IsDefault { get; set; } = false;
}
