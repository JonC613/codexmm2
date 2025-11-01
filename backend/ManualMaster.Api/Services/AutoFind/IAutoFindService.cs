using ManualMaster.Api.Dtos;

namespace ManualMaster.Api.Services.AutoFind;

public interface IAutoFindService
{
    Task<IReadOnlyList<AutoFindResultDto>> SearchAsync(AutoFindRequest request, CancellationToken cancellationToken);
}
