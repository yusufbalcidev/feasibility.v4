using System.Globalization;
using System.Text.Json;
using feasibility.Business.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace feasibility.Business.Concrete;

public class EvdsInflationService : IEvdsInflationService
{
    private const string SeriesCode  = "TP.FG.J0";
    private const string SeriesField = "TP_FG_J0";
    private const string CacheKey    = "evds_tufe_tr_annual";
    private static readonly TimeSpan CacheSuresi = TimeSpan.FromHours(24);

    private readonly HttpClient   _http;
    private readonly IMemoryCache _cache;
    private readonly string       _baseUrl;
    private readonly string?      _apiKey;

    public EvdsInflationService(HttpClient http, IMemoryCache cache, IConfiguration configuration)
    {
        _http    = http;
        _cache   = cache;
        _baseUrl = configuration["EvdsApi:BaseUrl"] ?? "https://evds3.tcmb.gov.tr/igmevdsms-dis/";
        _apiKey  = configuration["EvdsApi:ApiKey"];
    }

    public async Task<decimal?> GetLatestTufeAnnualAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKey, out decimal? cached))
            return cached;

        if (string.IsNullOrWhiteSpace(_apiKey))
            return null;

        try
        {
            var end   = DateTime.Now;
            var start = end.AddMonths(-14);
            var url =
                $"{_baseUrl}series={SeriesCode}" +
                $"&startDate={start:dd-MM-yyyy}&endDate={end:dd-MM-yyyy}" +
                "&type=json&formulas=3";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("key", _apiKey);

            using var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("items", out var items) ||
                items.ValueKind != JsonValueKind.Array)
                return null;

            decimal? result = null;
            foreach (var item in items.EnumerateArray())
            {
                if (!TryReadSeriesValue(item, out var value)) continue;
                result = Math.Round(value, 2);
            }

            if (result.HasValue)
                _cache.Set(CacheKey, result, CacheSuresi);

            return result;
        }
        catch
        {
            return null;
        }
    }

    private static bool TryReadSeriesValue(JsonElement item, out decimal value)
    {
        value = 0m;
        if (item.ValueKind != JsonValueKind.Object) return false;

        foreach (var prop in item.EnumerateObject())
        {
            if (!prop.Name.StartsWith(SeriesField, StringComparison.Ordinal)) continue;
            return TryReadDecimal(prop.Value, out value);
        }
        return false;
    }

    private static bool TryReadDecimal(JsonElement el, out decimal value)
    {
        value = 0m;
        return el.ValueKind switch
        {
            JsonValueKind.Number => el.TryGetDecimal(out value),
            JsonValueKind.String => decimal.TryParse(el.GetString(), NumberStyles.Any,
                                        CultureInfo.InvariantCulture, out value),
            _ => false
        };
    }
}
