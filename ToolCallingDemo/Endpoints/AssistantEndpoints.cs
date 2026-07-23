using Microsoft.Extensions.AI;
using ToolCallingDemo.Tools;

namespace ToolCallingDemo.Endpoints;

public record AssistantRequest(string Question);
public record AssistantReply(string Answer);

public static class AssistantEndpoints
{
    public static void MapAssistantEndpoints(this IEndpointRouteBuilder app) =>
        app.MapPost("/assistant", async (
            AssistantRequest req,
            IChatClient chat,
            OrderTools orderTools,
            PricingTools pricingTools,
            CancellationToken ct) =>
        {
            var options = new ChatOptions
            {
                Tools =
                [
                    AIFunctionFactory.Create(orderTools.GetOrderStatusAsync),
                    AIFunctionFactory.Create(pricingTools.ConvertPriceAsync)
                ]
            };

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System,
                    "You are a shop assistant. Use the tools for real data on orders and prices. " +
                    "Do not invent statuses or amounts: if a tool finds nothing, say so."),
                new(ChatRole.User, req.Question)
            };

            var response = await chat.GetResponseAsync(messages, options, ct);
            return Results.Ok(new AssistantReply(response.Text));
        });
}
