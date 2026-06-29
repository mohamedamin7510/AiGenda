namespace AI_genda_API.Abstractions.Const;

public static class OwnerPermissions
{
    public static string Type { get; } = nameof(Permissions);

    public const string AddWorkSpaces    = "workspaces:add"; 
    public const string GetWorkSpaces    = "workspaces:read"; 
    public const string UpdateWorkSpaces = "workspaces:update"; 
    public const string DeleteWorkSpaces = "workspaces:delete"; 


    public const string AddSpaces    = "spaces:add"; 
    public const string GetSpaces    = "spaces:read"; 
    public const string UpdateSpaces = "spaces:update"; 
    public const string DeleteSpaces = "spaces:delete"; 


    public const string AddTasks    = "tasks:add"; 
    public const string GetTasks    = "tasks:read"; 
    public const string UpdateTasks = "tasks:update"; 
    public const string DeleteTasks = "tasks:delete";


    public const string AddNotes    = "notes:add"; 
    public const string GetNotes    = "notes:read"; 
    public const string UpdateNotes = "notes:update"; 
    public const string DeleteNotes = "notes:delete";


    public const string ChangeUsersPermissions = "users:changepermissions";

    public static List<string> GetAllPerimision => typeof(Permissions).GetFields()
        .Select(x=>x.GetValue(x) as string).ToList()!;
    
}
