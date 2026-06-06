using feasibility.Entity.Dtos.FeasibilityAmortization;

namespace feasibility.Business.Abstract;

public interface IFeasibilityAmortizationService
{
    Task<FeasibilityAmortizationCreateFormDto> GetCreateFormDataAsync(CancellationToken ct = default);
    Task<string> GetRatesJsonAsync(CancellationToken ct = default);
    Task<decimal?> GetSonTufeAsync(CancellationToken ct = default);
    Task<Guid> SaveStudyAsync(FeasibilityAmortizationSaveDto dto, CancellationToken ct = default);
    Task<List<FeasibilityAmortizationListDto>> GetListAsync(CancellationToken ct = default);
    Task<FeasibilityAmortizationDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task RestoreAsync(Guid id, CancellationToken ct = default);
    Task<string?> GetEditPreloadJsonAsync(Guid id, CancellationToken ct = default);
    Task UpdateStudyAsync(Guid id, FeasibilityAmortizationSaveDto dto, CancellationToken ct = default);
}
