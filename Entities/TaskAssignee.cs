using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("TaskAssignees")]
public class TaskAssignee
{
    public string TaskId { get; set; } 
    public string UserId { get; set; }
    public DateTime AssignedAt { get; set; }




    [ForeignKey("TaskId")]
    public virtual Task? Task { get; set; }

    [ForeignKey("UserId")]
    public virtual ExtendedUser? User { get; set; }
}