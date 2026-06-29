using AI_genda_API.Abstractions.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("FocusSessions")]
public class FocusSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int WorkspaceId { get; set; }
    public string SpaceId { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string AmbientSound { get; set; } = string.Empty;
    public bool BreakAfter { get; set; }
    public bool BlockNotifications { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public FocusSessionStatus Status { get; set; } = FocusSessionStatus.Active;

    public int Interruptions { get; set; } = 0;
    public int InitialCompletedSubtasks { get; set; } = 0;
    public int InitialTotalSubtasks { get; set; } = 0;
    public int CompletedSubtasks { get; set; } = 0;
    public int TotalSubtasks { get; set; } = 0;

    public DateTime? PausedAt { get; set; }
    public int TotalPausedSeconds { get; set; } = 0;

    public virtual Space Space { get; set; } = default!;
}


