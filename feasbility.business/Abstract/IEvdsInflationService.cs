namespace feasibility.Business.Abstract;

public interface IEvdsInflationService
{
    /// <summary>
    /// TCMB EVDS'ten Türkiye TÜFE yıllık değişimini (%) döndürür. Veri yoksa null.
    /// </summary>
    Task<decimal?> GetLatestTufeAnnualAsync(CancellationToken ct = default);
}
