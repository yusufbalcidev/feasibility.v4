using feasibility.Business.Abstract;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;

namespace feasibility.Business.Concrete;

public class TcmbService : ITcmbService
{
    private const string TcmbUrl  = "https://www.tcmb.gov.tr/kurlar/today.xml";
    private const string CacheKey = "tcmb_rates";
    private static TimeSpan CacheSuresi => DateTime.Now.Date.AddDays(1) - DateTime.Now;

    private readonly HttpClient   _http;
    private readonly IMemoryCache _cache;

    public TcmbService(HttpClient http, IMemoryCache cache)
    {
        _http  = http;
        _cache = cache;
    }

    public async Task<TcmbRates> GetRatesAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKey, out TcmbRates? cached) && cached is not null)
            return cached;

        var xml  = await _http.GetStringAsync(TcmbUrl, ct);
        var doc  = XDocument.Parse(xml);

        var rates = new TcmbRates
        {
            UsdBuy  = ParseRate(doc, "USD", "ForexBuying"),
            UsdSell = ParseRate(doc, "USD", "ForexSelling"),
            EurBuy  = ParseRate(doc, "EUR", "ForexBuying"),
            EurSell = ParseRate(doc, "EUR", "ForexSelling"),
        };

        _cache.Set(CacheKey, rates, CacheSuresi);
        return rates;
    }

    private static decimal ParseRate(XDocument doc, string currencyCode, string elementName)
    {
        var value = doc.Descendants("Currency")
            .FirstOrDefault(e => (string?)e.Attribute("Kod") == currencyCode)
            ?.Element(elementName)?.Value;

        return decimal.TryParse(value, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result)
            ? result
            : 0m;
    }
}
