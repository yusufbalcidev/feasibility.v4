namespace feasibility.Entity.Dtos.FeasibilityAmortization;

public class FeasibilityAmortizationCreateFormDto
{
    public List<LocationSelectItem> Locations { get; init; } = new();
    public string FxJson { get; init; } = "{\"usdBuy\":0,\"usdSell\":0,\"eurBuy\":0,\"eurSell\":0}";
}

public class LocationSelectItem
{
    public string Value { get; init; } = string.Empty;
    public string Text  { get; init; } = string.Empty;
}
