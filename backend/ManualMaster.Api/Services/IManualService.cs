using ManualMaster.Api.Dtos;

namespace ManualMaster.Api.Services;

public interface IManualService
{
    Task<(IReadOnlyList<ManualSummaryDto> Items, int Total)> GetManualsAsync(ManualQueryParameters query, CancellationToken cancellationToken);
    Task<ManualDetailDto?> GetManualAsync(int id, CancellationToken cancellationToken);
    Task<(byte[] Data, string FileName, string ContentType)?> GetManualFileAsync(int id, CancellationToken cancellationToken);
    Task<ManualDetailDto> CreateManualAsync(ManualCreateRequest request, CancellationToken cancellationToken);
    Task<ManualDetailDto?> UpdateManualAsync(int id, ManualUpdateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteManualAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken);
}
