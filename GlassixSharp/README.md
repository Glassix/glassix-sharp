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
// Create credentials
var credentials = new GlassixCredentials(
    workspaceName: "your-workspace", 
    userName: "your-email@example.com", 
    apiKey: Guid.Parse("your-api-key"), 
    apiSecret: "your-api-secret",
    timeoutSeconds: 120 // Optional, defaults to 60 seconds
);

// Initialize the client
var client = new GlassixSharp(credentials);
```

## Examples

### Creating a Ticket

```csharp
var request = new CreateTicketRequest
{
    Field1 = "Customer Support Request",
    Culture = "en-US",
    Participants = new List<CreateTicketParticipant>
    {
        new CreateTicketParticipant
        {
            Name = "John Doe",
            ProtocolType = ProtocolType.WhatsApp.ToString(),
            Identifier = "1234567890"
        }
    }
};

var (success, ticket, error) = await client.CreateTicketAsync(request);

if (success)
{
    Console.WriteLine($"Ticket created with ID: {ticket.Id}");
}
else
{
    Console.WriteLine($"Failed to create ticket: {error}");
}
```

### Sending a Message in a Ticket

```csharp
var request = new SendMessageRequest
{
    Text = "Hello! How can I help you today?",
    Html = "<p>Hello! How can I help you today?</p>"
};

var (success, transaction, error) = await client.SendMessageAsync(12345, request);

if (success)
{
    Console.WriteLine($"Message sent with transaction ID: {transaction.Id}");
}
```

### Getting a Ticket

```csharp
var (success, ticket, error) = await client.GetTicketAsync(12345);

if (success)
{
    Console.WriteLine($"Retrieved ticket: {ticket.Field1}");
}
```

### Listing Tickets

```csharp
var since = DateTime.UtcNow.AddDays(-7);
var until = DateTime.UtcNow;

var (success, ticketList, error) = await client.ListTicketsAsync(
    since, 
    until, 
    ticketState: TicketState.Open,
    sortOrder: SortOrder.Descending
);

if (success)
{
    foreach (var ticket in ticketList.Tickets)
    {
        Console.WriteLine($"Ticket ID: {ticket.Id}, Subject: {ticket.Field1}");
    }
}
```

### Setting Ticket State

```csharp
var (success, response, error) = await client.SetTicketStateAsync(
    12345,
    nextState: TicketState.Closed
);

if (success)
{
    Console.WriteLine("Ticket closed successfully");
}
```

### Working with Users

```csharp
// Get all users
var (success, users, error) = await client.GetAllUsersAsync();

if (success)
{
    foreach (var user in users)
    {
        Console.WriteLine($"User: {user.FullName}, Email: {user.UserName}");
    }
}

// Set user status
await client.SetUserStatusAsync(UserStatus.Online);

// Get user status
var (statusSuccess, status, statusError) = await client.GetUserStatusAsync();
if (statusSuccess)
{
    Console.WriteLine($"Current status: {status.Status}");
}
```

### Sending Protocol Messages

```csharp
var request = new SendProtocolMessageRequest
{
    Text = "Hello from GlassixSharp!",
    ProtocolType = ProtocolType.WhatsApp.ToString(),
    From = "1234567890",
    To = "9876543210"
};

var (success, message, error) = await client.SendProtocolMessageAsync(request);
```

## Error Handling

All API methods return a tuple with:
1. A success boolean
2. The response data (null if failed)
3. An error message (null if succeeded)

```csharp
var (success, data, error) = await client.SomeMethodAsync();

if (!success)
{
    // Handle the error
    Console.WriteLine($"Error: {error}");
    return;
}

// Continue with successful response
```

## Enums for Type Safety

GlassixSharp provides enums for common values:

- `TicketState`: Open, Closed, Pending, Snoozed
- `UserStatus`: Online, Offline, Break, etc.
- `SortOrder`: Ascending, Descending
- `ProtocolType`: WhatsApp, SMS, Web, etc.

Using these enums helps prevent errors from typos and provides IntelliSense in your IDE.
# GlassixSharp

A simple C# SDK for the Glassix API. This SDK makes it easy to interact with the Glassix messaging platform.

## Features

- Automatic token management
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
var client = new GlassixSharp(
    workspaceName: "your-workspace", 
    userName: "your-email@example.com", 
    apiKey: Guid.Parse("your-api-key"), 
    apiSecret: "your-api-secret"
);
```

## Examples

### Creating a Ticket

```csharp
var request = new CreateTicketRequest
{
    Field1 = "Customer Support Request",
    Culture = "en-US",
    Participants = new List<CreateTicketParticipant>
    {
        new CreateTicketParticipant
        {
            Name = "John Doe",
            ProtocolType = "WhatsApp",
            Identifier = "1234567890"
        }
    }
};

var (success, ticket, error) = await client.CreateTicketAsync(request);

if (success)
{
    Console.WriteLine($"Ticket created with ID: {ticket.Id}");
}
else
{
    Console.WriteLine($"Failed to create ticket: {error}");
}
```

### Sending a Message in a Ticket

```csharp
var request = new SendMessageRequest
{
    Text = "Hello! How can I help you today?",
    Html = "<p>Hello! How can I help you today?</p>"
};

var (success, transaction, error) = await client.SendMessageAsync(12345, request);

if (success)
{
    Console.WriteLine($"Message sent with transaction ID: {transaction.Id}");
}
```

### Getting a Ticket

```csharp
var (success, ticket, error) = await client.GetTicketAsync(12345);

if (success)
{
    Console.WriteLine($"Retrieved ticket: {ticket.Field1}");
}
```

### Listing Tickets

```csharp
var since = DateTime.UtcNow.AddDays(-7);
var until = DateTime.UtcNow;

var (success, ticketList, error) = await client.ListTicketsAsync(since, until, ticketState: "Open");

if (success)
{
    foreach (var ticket in ticketList.Tickets)
    {
        Console.WriteLine($"Ticket ID: {ticket.Id}, Subject: {ticket.Field1}");
    }
}
```

### Setting Ticket State

```csharp
var (success, response, error) = await client.SetTicketStateAsync(
    12345,
    nextState: "Closed"
);

if (success)
{
    Console.WriteLine("Ticket closed successfully");
}
```

### Working with Users

```csharp
// Get all users
var (success, users, error) = await client.GetAllUsersAsync();

if (success)
{
    foreach (var user in users)
    {
        Console.WriteLine($"User: {user.FullName}, Email: {user.UserName}");
    }
}

// Set user status
await client.SetUserStatusAsync("Online");

// Get user status
var (statusSuccess, status, statusError) = await client.GetUserStatusAsync();
if (statusSuccess)
{
    Console.WriteLine($"Current status: {status.Status}");
}
```

### Sending Protocol Messages

```csharp
var request = new SendProtocolMessageRequest
{
    Text = "Hello from GlassixSharp!",
    ProtocolType = "WhatsApp",
    From = "1234567890",
    To = "9876543210"
};

var (success, message, error) = await client.SendProtocolMessageAsync(request);
```

## Error Handling

All API methods return a tuple with:
1. A success boolean
2. The response data (null if failed)
3. An error message (null if succeeded)

```csharp
var (success, data, error) = await client.SomeMethodAsync();

if (!success)
{
    // Handle the error
    Console.WriteLine($"Error: {error}");
    return;
}

// Continue with successful response
```

## Supported Features

- Ticket management (create, get, list, update)
- Message sending and receiving
- User management
- Contact management
- Protocol messaging (WhatsApp, SMS, etc.)
- Webhook event handling

