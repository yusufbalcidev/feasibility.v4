using System.Globalization;
using System.Text.Json;
using feasibility.Business.Abstract;
using feasibility.Entity.Dtos.Evds;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace feasibility.Business.Concrete;

public class EvdsService : IEvdsService
{
    private const string CachePrefix = "evds_";
    // TÜFE/ÜFE aylık yayınlanır — 24 saat cache yeterli
    private static readonly TimeSpan CacheSuresi = TimeSpan.FromHours(24);

    private readonly HttpClient   _http;
    private readonly IMemoryCache _cache;
    private readonly EvdsSettings _ayarlar;

    public EvdsService(HttpClient http, IMemoryCache cache, IOptions<EvdsSettings> ayarlar)
    {
        _http    = http;
        _cache   = cache;
        _ayarlar = ayarlar.Value;
    }

    public async Task<List<InflationDataPointDto>> GetInflationAsync(
        string seriKodu  = "TP.FG.J0",
        string baslangic = "01-01-2020",
        string? bitis    = null,
        int    frekans   = 5,
        string formul    = "4",
        CancellationToken ct = default)
    {
        bitis ??= DateTime.Now.ToString("dd-MM-yyyy");
        var cacheKey = $"{CachePrefix}{seriKodu}_{baslangic}_{bitis}_{frekans}_{formul}";

        if (_cache.TryGetValue(cacheKey, out List<InflationDataPointDto>? cached) && cached is not null)
            return cached;

        var url = $"{_ayarlar.BaseUrl}series={seriKodu}" +
                  $"&startDate={baslangic}" +
                  $"&endDate={bitis}" +
                  $"&type=json" +
                  $"&frequency={frekans}" +
                  $"&formulas={formul}";

        using var istek = new HttpRequestMessage(HttpMethod.Get, url);
        istek.Headers.Add("key", _ayarlar.ApiKey);

        var yanit = await _http.SendAsync(istek, ct);
        yanit.EnsureSuccessStatusCode();

        var jsonStr = await yanit.Content.ReadAsStringAsync(ct);
        var root    = JsonSerializer.Deserialize<EvdsYanit>(jsonStr);
        var sonuc   = ParseYanit(root);

        _cache.Set(cacheKey, sonuc, CacheSuresi);
        return sonuc;
    }

    private static List<InflationDataPointDto> ParseYanit(EvdsYanit? yanit)
    {
        var liste = new List<InflationDataPointDto>();
        if (yanit?.items is null) return liste;

        foreach (var item in yanit.items)
        {
            if (!item.TryGetProperty("Tarih", out var tarihEl)) continue;
            if (!DateTime.TryParseExact(tarihEl.GetString(), "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var tarih)) continue;

            decimal? deger = null;
            foreach (var prop in item.EnumerateObject())
            {
                if (prop.Name is "Tarih" or "UNIXTIME") continue;
                if (prop.Value.ValueKind == JsonValueKind.String &&
                    decimal.TryParse(prop.Value.GetString(), NumberStyles.Any,
                        CultureInfo.InvariantCulture, out var parsed))
                    deger = parsed;
                break;
            }

            liste.Add(new InflationDataPointDto { Tarih = tarih, Deger = deger });
        }

        return liste;
    }

    private sealed class EvdsYanit
    {
        public int totalCount { get; set; }
        public List<JsonElement> items { get; set; } = new();
    }
}
