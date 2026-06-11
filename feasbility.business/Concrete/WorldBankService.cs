using System.Text.Json;
using feasibility.Business.Abstract;
using Microsoft.Extensions.Caching.Memory;

namespace feasibility.Business.Concrete;

public class WorldBankService : IWorldBankService
{
    private const string UrlTemplate = "https://api.worldbank.org/v2/country/{0}/indicator/FP.CPI.TOTL.ZG?format=json&mrv=3";
    private static readonly TimeSpan CacheSuresi = TimeSpan.FromHours(48);
    private const string CachePrefix = "wb_inf_";

    private readonly HttpClient   _http;
    private readonly IMemoryCache _cache;

    public WorldBankService(HttpClient http, IMemoryCache cache)
    {
        _http  = http;
        _cache = cache;
    }

    public async Task<decimal?> GetLatestInflationAsync(string countryCode, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}{countryCode}";
        if (_cache.TryGetValue(cacheKey, out decimal? cached))
            return cached;

        try
        {
            var url  = string.Format(UrlTemplate, countryCode);
            var json = await _http.GetStringAsync(url, ct);

            using var doc  = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() < 2)
                return null;

            var data = root[1];
            if (data.ValueKind != JsonValueKind.Array)
                return null;

            decimal? result = null;
            foreach (var item in data.EnumerateArray())
            {
                if (!item.TryGetProperty("value", out var valEl)) continue;
                if (valEl.ValueKind == JsonValueKind.Null) continue;

                result = Math.Round((decimal)valEl.GetDouble(), 2);
                break;
            }

            if (result.HasValue)
                _cache.Set(cacheKey, result, CacheSuresi);

            return result;
        }
        catch
        {
            return null;
        }
    }
}
