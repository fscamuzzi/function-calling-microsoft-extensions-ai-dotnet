# Function Calling in .NET — Microsoft.Extensions.AI + Ollama

Full working code for the tutorial **"Function Calling in .NET: let an LLM call your own code with Microsoft.Extensions.AI"**: a Minimal API shop assistant where a local LLM (Ollama) calls your C# methods as tools — order status from MongoDB, currency conversion via an external API (Refit).

- 📄 Blog article: [Function Calling in .NET with Microsoft.Extensions.AI](https://cool-solution.com/en/blog/function-calling-microsoft-extensions-ai-dotnet)
- 💼 LinkedIn article: [Let a local LLM call your own C# code — function calling](https://www.linkedin.com/pulse/let-local-llm-call-your-own-c-code-function-calling-federico-scamuzzi-57hef/)

## What's inside

- **Microsoft.Extensions.AI** `IChatClient` + `UseFunctionInvocation()`: the runtime executes your C# tools and loops until the model has an answer
- **`AIFunctionFactory.Create`** over plain methods with `[Description]` attributes
- **OllamaSharp** as the `IChatClient` provider (swap for OpenAI/Azure with one registration line)
- **MongoDB** tool (`OrderTools.GetOrderStatusAsync`) and **Refit** tool (`PricingTools.ConvertPriceAsync`)
- Safety limits: max iterations / max consecutive errors per request

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Ollama](https://ollama.com) with a tool-calling model: `ollama pull llama3.1`
- MongoDB on `localhost:27017` (`docker run -d -p 27017:27017 mongo` works fine)

## Run it

```bash
# terminal 1 — Ollama
ollama serve            # usually already running on http://localhost:11434

# terminal 2 — the API
cd ToolCallingDemo
dotnet build
dotnet run --urls http://localhost:5000
```

Seed a demo order (optional, for the MongoDB tool):

```bash
docker exec -i $(docker ps -qf ancestor=mongo) mongosh shopdemo --eval \
  'db.orders.insertOne({Code:"ORD-1024",Status:"shipped",Total:NumberDecimal("149.90"),EstimatedDelivery:"2026-07-30"})'
```

## Try it

```bash
# order status → GetOrderStatusAsync (MongoDB)
curl http://localhost:5000/assistant \
    -H "Content-Type: application/json" \
    -d '{"question":"Where is order ORD-1024?"}'

# currency conversion → ConvertPriceAsync (exchange-rate API)
curl http://localhost:5000/assistant \
    -H "Content-Type: application/json" \
    -d '{"question":"How much is 149.90 EUR in USD?"}'
```

## Project structure

```
ToolCallingDemo/
├── Program.cs
├── appsettings.json                      # Ollama / Mongo / ExchangeRates config
├── Extensions/AssistantServiceExtensions.cs
├── Tools/OrderTools.cs                   # MongoDB tool + Order model
├── Tools/PricingTools.cs                 # Refit exchange-rate tool
└── Endpoints/AssistantEndpoints.cs       # POST /assistant
```
