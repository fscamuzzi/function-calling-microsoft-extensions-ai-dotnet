using System.ComponentModel;
using Refit;

namespace ToolCallingDemo.Tools;

public interface IExchangeRateApi
{
    [Get("/convert")]
    Task<ConvertResponse> ConvertAsync(string from, string to, decimal amount, CancellationToken ct = default);
}

public record ConvertResponse(decimal Result);

public class PricingTools(IExchangeRateApi api)
{
    [Description("Converts an amount from one currency to another at the current exchange rate.")]
    public async Task<decimal> ConvertPriceAsync(
        [Description("Amount to convert")] decimal amount,
        [Description("Source currency ISO 4217, e.g. EUR")] string from,
        [Description("Target currency ISO 4217, e.g. USD")] string to,
        CancellationToken ct = default)
    {
        var conversion = await api.ConvertAsync(from, to, amount, ct);
        return Math.Round(conversion.Result, 2);
    }
}
