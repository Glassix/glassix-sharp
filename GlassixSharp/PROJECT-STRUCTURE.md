# GlassixSharp Project Structure

This document outlines the organization of the GlassixSharp SDK.

## Core Structure

```
GlassixSharp/
├── GlassixSharp.cs              # Main SDK client class
├── Models/                      # Data models
│   ├── ApiResponse.cs           # Generic API response wrapper
│   ├── Attachment.cs            # Attachment model
│   ├── Contact.cs               # Contact and ContactIdentifier models
│   ├── Participant.cs           # Participant model
│   ├── Ticket.cs                # Ticket model
│   ├── TicketDetails.cs         # TicketDetails and related models
│   ├── Transaction.cs           # Transaction model
│   ├── User.cs                  # User model
│   ├── WebhookEvent.cs          # WebhookEvent models
│   ├── Requests/                # Request models
│   │   ├── CreateTicketRequest.cs
│   │   ├── SendMessageRequest.cs
│   │   ├── SendProtocolMessageRequest.cs
│   │   ├── SetTicketFieldsRequest.cs
│   │   ├── SetTicketStateRequest.cs
│   │   └── TokenRequest.cs
│   └── Responses/               # Response models
│       ├── EmptyResponse.cs
│       ├── ErrorResponse.cs
│       ├── MessageResponse.cs
│       ├── SendProtocolMessageResponse.cs
│       ├── TicketListResponse.cs
│       ├── TicketResponse.cs
│       ├── TokenResponse.cs
│       ├── TransactionResponse.cs
│       └── UserStatusResponse.cs
└── Utilities/                   # Helper classes
    ├── GlassixApiException.cs   # Custom exception
    └── QueryStringBuilder.cs    # Helper for building query strings
```

## Namespaces

- `GlassixSharp`: Root namespace containing the main client class
- `GlassixSharp.Models`: Contains all data models
- `GlassixSharp.Models.Requests`: Contains request models sent to the API
- `GlassixSharp.Models.Responses`: Contains response models received from the API
- `GlassixSharp.Utilities`: Contains helper classes and utilities

## Design Patterns

1. **Singleton Pattern**: Static `HttpClient` to avoid socket exhaustion
2. **Repository Pattern**: The main `GlassixSharp` class acts as a repository for all API operations
3. **Builder Pattern**: Request classes use a builder-like pattern with property setters

## Thread Safety

- Token management is thread-safe using `SemaphoreSlim`
- Concurrent access to the token dictionary is handled with `ConcurrentDictionary`

## Error Handling

- All API methods return tuples with success boolean, data, and error message
- The `GlassixApiException` is thrown for severe errors
- The `ApiResponse<T>` class encapsulates API responses with success/error information
