using feasibility.Entity.Dtos.FeasibilityAmortization;

namespace feasibility.Business.Abstract;

public interface IFeasibilityAmortizationService
{
    Task<FeasibilityAmortizationCreateFormDto> GetCreateFormDataAsync(CancellationToken ct = default);
    Task<string> GetRatesJsonAsync(CancellationToken ct = default);
    Task<(decimal? tl, decimal? usd, decimal? eur)> GetSonEnflasyonlarAsync(CancellationToken ct = default);
    Task<Guid> SaveStudyAsync(FeasibilityAmortizationSaveDto dto, CancellationToken ct = default);
    Task<FeasibilityAmortizationDetailDto> CalculatePreviewAsync(FeasibilityAmortizationSaveDto dto, CancellationToken ct = default);
    Task<List<FeasibilityAmortizationListDto>> GetListAsync(CancellationToken ct = default);
    Task<FeasibilityAmortizationDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task RestoreAsync(Guid id, CancellationToken ct = default);
    Task<string?> GetEditPreloadJsonAsync(Guid id, CancellationToken ct = default);
    Task UpdateStudyAsync(Guid id, FeasibilityAmortizationSaveDto dto, CancellationToken ct = default);

    Task<bool> IsLatestVersionAsync(Guid id, CancellationToken ct = default);
}
