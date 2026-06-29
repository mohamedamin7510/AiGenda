using AI_genda_API.Contracts.Common;
using AI_genda_API.Contracts.Note;

namespace AI_genda_API.Services.NoteService;

public interface INoteService
{
    Task<Result<NoteResponse>> AddAsync(int WorkspaceId, string SpaceId, string UserId, NoteRequest request, CancellationToken cancellationToken = default!);
    Task<Result<PaginatedList<NoteResponse>>> GetAllAsync(int WorkspaceId, string SpaceId, string UserId, FilterRequest filterRequest, CancellationToken cancellationToken = default!);
    Task<Result<NoteResponse>> GetByIdAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<NoteResponse>> UpdateAsync(int WorkspaceId, string SpaceId, string Id, string UserId, NoteRequest request, CancellationToken cancellationToken = default!);
    Task<Result> DeleteAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!);
}