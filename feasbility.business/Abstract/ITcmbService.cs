namespace feasibility.Business.Abstract;

public interface ITcmbService
{
    Task<TcmbRates> GetRatesAsync(CancellationToken ct = default);
}

public class TcmbRates
{
    public decimal UsdBuy  { get; init; }
    public decimal UsdSell { get; init; }
    public decimal EurBuy  { get; init; }
    public decimal EurSell { get; init; }
}
