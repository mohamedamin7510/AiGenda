using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("SubTasks")]
public class SubTask : AuditLogging
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public string TaskId { get; set; } = string.Empty;



    [ForeignKey("TaskId")]
    public virtual Task? Task { get; set; }
}