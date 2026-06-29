using AI_genda_API.Abstractions.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("Tasks")]
public class Task : AuditLogging
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public TaskStatuss Status { get; set; } = TaskStatuss.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public string SpaceId { get; set; } = string.Empty;





    [ForeignKey("SpaceId")]
    public virtual Space Space { get; set; } = null!;
    public virtual List<TaskAssignee> TaskAssignees { get; set; } = [];
    public virtual List<SubTask> SubTasks { get; set; } = [];
}