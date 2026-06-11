namespace feasibility.Business.Abstract;

public interface IEvdsInflationService
{
    Task<decimal?> GetLatestTufeAnnualAsync(CancellationToken ct = default);
}
