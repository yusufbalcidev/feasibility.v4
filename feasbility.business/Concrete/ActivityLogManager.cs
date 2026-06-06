using AutoMapper;
using feasibility.Business.Abstract;
using feasibility.DataAccess.Abstract;
using feasibility.Entity.Dtos.ActivityLog;
using feasibility.Entity.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace feasibility.Business.Concrete;

public class ActivityLogManager : GenericManager<ActivityLog>, IActivityLogService
{
    private readonly IGenericRepository<ActivityLog> _repository;
    private readonly IMapper _mapper;

    public ActivityLogManager(IGenericRepository<ActivityLog> repository, IMapper mapper) : base(repository)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ActivityLogListDto>> GetListAsync(int take = 500, CancellationToken ct = default)
    {
        return await _repository.Query()
            .OrderByDescending(l => l.CreatedAt)
            .Take(take)
            .Select(l => new ActivityLogListDto
            {
                Id = l.Id,
                CreatedAt = l.CreatedAt,
                UserName = l.UserName,
                UserEmail = l.UserEmail,
                HttpMethod = l.HttpMethod,
                Path = l.Path,
                Controller = l.Controller,
                Action = l.Action,
                StatusCode = l.StatusCode,
                ElapsedMs = l.ElapsedMs,
                IpAddress = l.IpAddress,
                ActionType = l.ActionType,
                Description = l.Description,
                ErrorMessage = l.ErrorMessage
            })
            .ToListAsync(ct);
    }

    public async Task<ActivityLogDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct: ct);
        return entity is null ? null : _mapper.Map<ActivityLogDetailDto>(entity);
    }

    public async Task LogAsync(ActivityLog log, CancellationToken ct = default)
    {
        await _repository.AddAsync(log, ct);
        await _repository.SaveChangesAsync(ct);
    }
}
