using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Entities;

public class WorkSpace:AuditLogging
{
    public int Id { get; set; }
    public  string Name { get; set; }
    public  string?  Description { get; set; }
    public string? IconCode { get; set; }
    public bool IsActive { get; set; } = true;
    public WorkSpaceVisibility Visibility { get; set; } = WorkSpaceVisibility.Private;

    public virtual List<WorkspaceMember> workspaceMembers { get; set; } = [];
    public virtual List<Space> Spaces { get; set; } = [];


}
