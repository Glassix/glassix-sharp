# GlassixSharp

A simple C# SDK for the Glassix API. This SDK makes it easy to interact with the Glassix messaging platform.

## Features

- Automatic token management with proper caching
- Strongly typed request and response models
- Simple error handling with tuples
- Thread-safe implementation
- Support for all major Glassix API endpoints
- Modular client architecture with specialized clients for different API areas

## Installation

```
dotnet add package GlassixSharp
```

## Getting Started

Initialize the clients with your Glassix credentials:

```csharp
Credentials credentials = new Credentials(
    workspaceName: "your-workspace", 
    userName: "your-email@example.com", 
    apiKey: Guid.Parse("your-api-key"), 
    apiSecret: "your-api-secret",
    timeoutSeconds: 120 // Optional, defaults to 60 seconds
);

// Initialize clients for different API areas
TicketsClient ticketsClient = new TicketsClient(credentials);
UsersClient usersClient = new UsersClient(credentials);
ContactsClient contactsClient = new ContactsClient(credentials);
ProtocolsClient protocolsClient = new ProtocolsClient(credentials);
WebhooksClient webhooksClient = new WebhooksClient(credentials);
```

## Examples

### Working with Tickets

```csharp
// Creating a Ticket
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

var (success, ticket, error) = await ticketsClient.CreateTicketAsync(request);

// Sending a Message in a Ticket
SendMessageRequest messageRequest = new SendMessageRequest
{
    Text = "Hello! How can I help you today?"
};

var (msgSuccess, transaction, msgError) = await ticketsClient.SendMessageAsync(12345, messageRequest);

// Getting a Ticket
var (getSuccess, retrievedTicket, getError) = await ticketsClient.GetTicketAsync(12345);

// Listing Tickets
var since = DateTime.UtcNow.AddDays(-7);
var until = DateTime.UtcNow;

var (listSuccess, ticketList, listError) = await ticketsClient.ListTicketsAsync(
    since, 
    until, 
    ticketState: Ticket.State.Open,
    sortOrder: SortOrder.Descending
);

// Setting Ticket State
var (stateSuccess, stateResponse, stateError) = await ticketsClient.SetTicketStateAsync(
    12345,
    nextState: Ticket.State.Closed
);

// Adding a note to a ticket
var (noteSuccess, noteError) = await ticketsClient.AddNoteAsync(
    12345,
    text: "Internal note about this customer"
);

// Setting ticket fields
var (fieldsSuccess, fieldsError) = await ticketsClient.SetTicketFieldsAsync(
    12345,
    new SetTicketFieldsRequest { /* fields to update */ }
);
```

### Working with Users

```csharp
// Get all users
var (usersSuccess, users, usersError) = await usersClient.GetAllUsersAsync();

// Set user status
var (setStatusSuccess, setStatusError) = await usersClient.SetUserStatusAsync(User.UserStatus.Online);

// Get user status
var (statusSuccess, status, statusError) = await usersClient.GetUserStatusAsync();
```

### Working with Contacts

```csharp
// Get a contact
var (contactSuccess, contact, contactError) = await contactsClient.GetContactAsync(Guid.Parse("contact-guid-here"));

// Set contact name
var (nameSuccess, nameResponse, nameError) = await contactsClient.SetContactNameAsync(
    Guid.Parse("contact-guid-here"),
    "New Contact Name"
);
```

### Sending Protocol Messages

```csharp
var message = new Message
{
    Text = "Hello from GlassixSharp!",
    ProtocolType = ProtocolType.WhatsApp,
    From = "15551234567",
    To = "15559876543"
};

var (success, sentMessage, error) = await protocolsClient.SendProtocolMessageAsync(message);
```

### Working with Webhooks

```csharp
// Get webhook events
var (success, events, error) = await webhooksClient.GetWebhookEventsAsync(deleteEvents: true);
```