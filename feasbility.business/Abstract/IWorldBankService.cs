namespace feasibility.Business.Abstract;

public interface IWorldBankService
{
    Task<decimal?> GetLatestInflationAsync(string countryCode, CancellationToken ct = default);
}
