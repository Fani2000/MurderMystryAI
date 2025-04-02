using Backend.GraphQL.Mutations;
using Backend.GraphQL.Queries;
using Backend.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add KeyVaultService to DI container
var keyVaultUrl = builder.Configuration["AzureAI:KeyVaultUrl"]; // Get Key Vault URL from app settings

builder.Services.AddSingleton(new KeyVaultService(keyVaultUrl));

// Add GraphQL services
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

// Build the app
var app = builder.Build();

app.MapGraphQL();
app.Run();