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
- [Docker](https://docs.docker.com/get-docker/) — MongoDB and Ollama run via `docker compose`, nothing else to install

Prefer native installs? [Ollama](https://ollama.com) (`ollama pull llama3.1`) + MongoDB on `localhost:27017` work exactly the same.

## Run the dependencies with Docker

One command starts everything the API needs — MongoDB (already seeded with the demo order `ORD-1024`) and Ollama (llama3.1 auto-pulled by the one-shot `ollama-init` service):

```bash
docker compose up -d

# first run only: watch the model download finish (~5 GB)
docker compose logs -f ollama-init
```

`appsettings.json` already points at `localhost:27017` and `localhost:11434`, so no configuration changes are needed. Stop everything with `docker compose down` (add `-v` to also wipe the model and the database).

## Run it

```bash
cd ToolCallingDemo
dotnet build
dotnet run --urls http://localhost:5000
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
docker-compose.yml                        # MongoDB (seeded) + Ollama + model pull
mongo-init/init-orders.js                 # seeds ORD-1024 on first start
ToolCallingDemo/
├── Program.cs
├── appsettings.json                      # Ollama / Mongo / ExchangeRates config
├── Extensions/AssistantServiceExtensions.cs
├── Tools/OrderTools.cs                   # MongoDB tool + Order model
├── Tools/PricingTools.cs                 # Refit exchange-rate tool
└── Endpoints/AssistantEndpoints.cs       # POST /assistant
```
