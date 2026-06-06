using feasibility.Entity.Dtos.ActivityLog;
using feasibility.Entity.Entities.Logging;

namespace feasibility.Business.Abstract;

public interface IActivityLogService : IGenericService<ActivityLog>
{
    Task<List<ActivityLogListDto>> GetListAsync(int take = 500, CancellationToken ct = default);
    Task<ActivityLogDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);
    Task LogAsync(ActivityLog log, CancellationToken ct = default);
}
