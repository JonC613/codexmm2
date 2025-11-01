using ManualMaster.Api.Models;

namespace ManualMaster.Api.Services;

public interface IManualRepository
{
    Task<IReadOnlyList<Manual>> GetManualsAsync(string? category, string? search, int page, int pageSize, CancellationToken cancellationToken);
    Task<int> CountManualsAsync(string? category, string? search, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<Manual?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Manual manual, CancellationToken cancellationToken);
    Task UpdateAsync(Manual manual, CancellationToken cancellationToken);
    Task DeleteAsync(Manual manual, CancellationToken cancellationToken);
}
