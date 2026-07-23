using Microsoft.Extensions.AI;
using MongoDB.Driver;
using OllamaSharp;
using Refit;
using ToolCallingDemo.Tools;

namespace ToolCallingDemo.Extensions;

public static class AssistantServiceExtensions
{
    public static IServiceCollection AddAssistant(this IServiceCollection services, IConfiguration config)
    {
        var ollama = config.GetSection("Ollama");
        var chatClient = new OllamaApiClient(new Uri(ollama["Endpoint"]!), ollama["ChatModel"]!);

        services.AddChatClient(chatClient)
            .UseFunctionInvocation()
            .UseLogging();

        var mongo = config.GetSection("Mongo");
        var mongoClient = new MongoClient(mongo["ConnectionString"]);
        services.AddSingleton<IMongoDatabase>(_ => mongoClient.GetDatabase(mongo["Database"]));

        services.AddRefitClient<IExchangeRateApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(config["ExchangeRates:BaseUrl"]!));

        services.AddScoped<OrderTools>();
        services.AddScoped<PricingTools>();
        return services;
    }
}
