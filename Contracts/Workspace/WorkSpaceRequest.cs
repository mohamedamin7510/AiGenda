using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Workspace;

public record WorkSpaceRequest
(
   string Name, 
     string? Description ,
         string? IconCode,
             WorkSpaceVisibility Visibility
);
