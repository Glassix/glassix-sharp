# GlassixSharp

A simple C# SDK for the Glassix API. This SDK makes it easy to interact with the Glassix messaging platform.

## Features

- Automatic token management with proper caching
- Strongly typed request and response models
- Simple error handling with tuples
- Thread-safe implementation
- Support for all major Glassix API endpoints

## Installation

```
dotnet add package GlassixSharp
```

## Getting Started

Initialize the client with your Glassix credentials:

```csharp
Credentials credentials = new Credentials(
    workspaceName: "your-workspace", 
    userName: "your-email@example.com", 
    apiKey: Guid.Parse("your-api-key"), 
    apiSecret: "your-api-secret",
    timeoutSeconds: 120 // Optional, defaults to 60 seconds
);

// Initialize the client
GlassixClient client = new GlassixClient(credentials);
```

## Examples

### Creating a Ticket

```csharp
CreateTicketRequest request = new CreateTicketRequest
{
    Field1 = "Customer Support Request",
    Culture = "en-US",
    Participants = new List<CreateTicketParticipant>
    {
        new CreateTicketParticipant
        {
            Name = "John Doe",
            ProtocolType = ProtocolType.WhatsApp,
            Identifier = "15551234567"
        }
    }
};

var (success, ticket, error) = await client.CreateTicketAsync(request);
```

### Sending a Message in a Ticket

```csharp
SendMessageRequest request = new SendMessageRequest
{
    Text = "Hello! How can I help you today?"
};

var (success, transaction, error) = await client.SendMessageAsync(12345, request);
```

### Getting a Ticket

```csharp
var (success, ticket, error) = await client.GetTicketAsync(12345);
```

### Listing Tickets

```csharp
var since = DateTime.UtcNow.AddDays(-7);
var until = DateTime.UtcNow;

var (success, ticketList, error) = await client.ListTicketsAsync(
    since, 
    until, 
    ticketState: Ticket.State.Open,
    sortOrder: SortOrder.Descending
);
```

### Setting Ticket State

```csharp
var (success, response, error) = await client.SetTicketStateAsync(
    12345,
    nextState: Ticket.State.Closed
);
```

### Working with Users

```csharp
// Get all users
var (success, users, error) = await client.GetAllUsersAsync();

// Set user status
await client.SetUserStatusAsync(User.UserStatus.Online);

// Get user status
var (statusSuccess, status, statusError) = await client.GetUserStatusAsync();
```

### Sending Protocol Messages

```csharp
var request = new SendProtocolMessageRequest
{
    Text = "Hello from GlassixSharp!",
    ProtocolType = ProtocolType.WhatsApp,
    From = "15551234567",
    To = "15559876543"
};

var (success, message, error) = await client.SendProtocolMessageAsync(request);
```

## Dependency Injection in ASP.NET Core

You can register the GlassixClient with the dependency injection container in ASP.NET Core:

```csharp
// Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Register Glassix client as a singleton
    services.AddSingleton<IGlassixClient>(serviceProvider => 
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        Credentials credentials = new Credentials(
            workspaceName: configuration["Glassix:WorkspaceName"],
            userName: configuration["Glassix:UserName"],
            apiKey: Guid.Parse(configuration["Glassix:ApiKey"]),
            apiSecret: configuration["Glassix:ApiSecret"],
            timeoutSeconds: int.Parse(configuration["Glassix:TimeoutSeconds"] ?? "60")
        );
        
        return new GlassixClient(credentials);
    });
    
    // Add other services...
}
```

## Usage in a Service Class

Here's an example of using the Glassix client in a service class:

```csharp
public class TicketService
{
    private readonly IGlassixClient _glassixClient;
    private readonly ILogger<TicketService> _logger;
    
    public TicketService(IGlassixClient glassixClient, ILogger<TicketService> logger)
    {
        _glassixClient = glassixClient;
        _logger = logger;
    }
}
```

## Error Handling

All API methods return a tuple with:
1. A success boolean
2. The response data (null if failed)
3. An error message (null if succeeded)