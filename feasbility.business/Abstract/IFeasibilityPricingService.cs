using feasibility.Entity.Dtos.FeasibilityPricing;

namespace feasibility.Business.Abstract;

public interface IFeasibilityPricingService
{
    Task<FeasibilityPricingCreateFormDto> GetCreateFormDataAsync(CancellationToken ct = default);
    Task<string> GetRatesJsonAsync(CancellationToken ct = default);
    Task<(decimal? tl, decimal? usd, decimal? eur)> GetSonEnflasyonlarAsync(CancellationToken ct = default);
    Task<Guid> SaveStudyAsync(FeasibilityPricingSaveDto dto, CancellationToken ct = default);
    Task<FeasibilityPricingDetailDto> CalculatePreviewAsync(FeasibilityPricingSaveDto dto, CancellationToken ct = default);
    Task<List<FeasibilityPricingListDto>> GetListAsync(CancellationToken ct = default);
    Task<FeasibilityPricingDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task RestoreAsync(Guid id, CancellationToken ct = default);
    Task<string?> GetEditPreloadJsonAsync(Guid id, CancellationToken ct = default);
    Task UpdateStudyAsync(Guid id, FeasibilityPricingSaveDto dto, CancellationToken ct = default);
    Task<bool> IsLatestVersionAsync(Guid id, CancellationToken ct = default);
}
