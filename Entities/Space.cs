using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

public class Space : AuditLogging
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IconCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = true;
    public DateOnly LastActivity { get; set; }
    public int WorkSpaceId { get; set; }

    [ForeignKey("WorkSpaceId")]
    public virtual WorkSpace WorkSpace { get; set; } = null!;
    public virtual List<Task> Tasks { get; set; } = [];
    public virtual List<Note> Notes { get; set; } = [];
    public virtual List<FocusSession> FocusSessions { get; set; } = [];
}