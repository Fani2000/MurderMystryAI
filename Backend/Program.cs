using Backend.GraphQL.Mutations;
using Backend.GraphQL.Queries;
using Backend.Validators;
using Backend.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add KeyVaultService to DI container
var keyVaultUrl = builder.Configuration["AzureAI:KeyVaultUrl"]; // Get Key Vault URL from app settings
builder.Services.AddSingleton(new KeyVaultService(keyVaultUrl));

// Add AzureAIOptions to DI container
builder.Services.AddSingleton<AzureAIOptions>();

// Add GraphQL services
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

// Configure CORS explicitly to allow the frontend origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Allow requests from frontend (React app)
            .AllowAnyHeader()                    // Allow any HTTP headers
            .AllowAnyMethod()                    // Allow all HTTP methods
            .AllowCredentials();                 // Allow credentials (cookies, headers like Authorization, etc.)
    });
});

var app = builder.Build();

// Enable CORS middleware with named policy
app.UseCors("CorsPolicy"); // Apply the "CorsPolicy" to the application

app.MapGraphQL();

app.Run();