using feasibility.Entity.Dtos.Evds;

namespace feasibility.Business.Abstract;

public interface IEvdsService
{
    /// <summary>
    /// Belirtilen EVDS seri kodunu çeker.
    /// Varsayılanlar: TP.FG.J0 (TÜFE Genel), aylık frekans, yıllık % değişim (formül 4).
    /// </summary>
    Task<List<InflationDataPointDto>> GetInflationAsync(
        string seriKodu  = "TP.FG.J0",
        string baslangic = "01-01-2020",
        string? bitis    = null,
        int    frekans   = 5,
        string formul    = "4",
        CancellationToken ct = default);
}
