# GlassixSharp

[![NuGet](https://img.shields.io/nuget/v/GlassixSharp.svg)](https://www.nuget.org/packages/GlassixSharp/)
[![License](https://img.shields.io/github/license/glassix/GlassixSharp.svg)](https://github.com/glassix/GlassixSharp/blob/main/LICENSE)
[![Build Status](https://img.shields.io/github/actions/workflow/status/glassix/GlassixSharp/build.yml?branch=main)](https://github.com/glassix/GlassixSharp/actions)
[![Downloads](https://img.shields.io/nuget/dt/GlassixSharp.svg)](https://www.nuget.org/packages/GlassixSharp/)

A simple C# SDK for the Glassix API. This SDK makes it easy to interact with the Glassix messaging platform.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Getting Started](#getting-started)
- [Examples](#examples)
  - [Working with Tickets](#working-with-tickets)
  - [Working with Users](#working-with-users)
  - [Working with Contacts](#working-with-contacts)
  - [Sending Protocol Messages](#sending-protocol-messages)
  - [Working with Webhooks](#working-with-webhooks)
  - [Working with Tenants](#working-with-tenants)
  - [Working with Canned Replies](#working-with-canned-replies)
- [Documentation](#documentation)
- [License](#license)

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
    apiSecret: "your-api-secret"
);

// Initialize clients for different API areas
TicketsClient ticketsClient = new TicketsClient(credentials);
UsersClient usersClient = new UsersClient(credentials);
ContactsClient contactsClient = new ContactsClient(credentials);
ProtocolsClient protocolsClient = new ProtocolsClient(credentials);
WebhooksClient webhooksClient = new WebhooksClient(credentials);
TenantsClient tenantsClient = new TenantsClient(credentials);
CannedRepliesClient cannedRepliesClient = new CannedRepliesClient(credentials);
```

## Examples

### Working with Tickets

```csharp
// Creating a Ticket
// Documentation: https://docs.glassix.com/reference/create-ticket
CreateTicketRequest request = new CreateTicketRequest
{
    field1 = "Customer Support Request",
    culture = "en-US",
    participants = new List<Participant>
    {
        new Participant
        {
            name = "John Doe",
            protocolType = ProtocolType.WhatsApp,
            identifier = "15551234567"
        }
    }
};

var (success, ticket, error) = await ticketsClient.CreateTicketAsync(request);

// Sending a Message in a Ticket
// Documentation: https://docs.glassix.com/reference/send-ticket
SendMessageRequest messageRequest = new SendMessageRequest
{
    text = "Hello! How can I help you today?"
};

var (msgSuccess, transaction, msgError) = await ticketsClient.SendMessageAsync(12345, messageRequest);

// Getting a Ticket
// Documentation: https://docs.glassix.com/reference/get-ticket
var (getSuccess, retrievedTicket, getError) = await ticketsClient.GetTicketAsync(12345);

// Listing Tickets
// Documentation: https://docs.glassix.com/reference/get-tickets-list
var since = DateTime.UtcNow.AddDays(-7);
var until = DateTime.UtcNow;

var (listSuccess, ticketList, listError) = await ticketsClient.ListTicketsAsync(
    since, 
    until, 
    ticketState: Ticket.State.Open,
    sortOrder: SortOrder.Descending
);

// Setting Ticket State
// Documentation: https://docs.glassix.com/reference/set-ticket-sate
var (stateSuccess, stateResponse, stateError) = await ticketsClient.SetTicketStateAsync(
    12345,
    nextState: Ticket.State.Closed
);

// Adding a note to a ticket
// Documentation: https://docs.glassix.com/reference/add-note
var (noteSuccess, noteError) = await ticketsClient.AddNoteAsync(
    12345,
    text: "Internal note about this customer"
);

// Setting ticket fields
// Documentation: https://docs.glassix.com/reference/setfields
var (fieldsSuccess, fieldsError) = await ticketsClient.SetTicketFieldsAsync(
    12345,
    new SetTicketFieldsRequest { /* fields to update */ }
);

// Adding tags to a ticket
// Documentation: https://docs.glassix.com/reference/add-tags
var (tagsSuccess, updatedTags, tagsError) = await ticketsClient.AddTicketTagsAsync(
    12345,
    new List<string> { "urgent", "follow-up" }
);

// Removing a tag from a ticket
// Documentation: https://docs.glassix.com/reference/remove-tag
var (removeTagSuccess, remainingTags, removeTagError) = await ticketsClient.RemoveTicketTagAsync(
    12345,
    "follow-up"
);

// Setting participant name
// Documentation: https://docs.glassix.com/reference/set-participant-name
var (setParticipantSuccess, setParticipantError) = await ticketsClient.SetParticipantNameAsync(
    12345,
    participantId: 1,
    name: "Updated Customer Name"
);

// Changing ticket owner
// Documentation: https://docs.glassix.com/reference/set-owner
var (ownerSuccess, ownerError) = await ticketsClient.SetTicketOwnerAsync(
    12345,
    nextOwnerUserName: "agent@example.com"
);

// Assigning an available user
// Documentation: https://docs.glassix.com/reference/assign-available-user
var (assignSuccess, assignError) = await ticketsClient.AssignAvailableUserAsync(12345);

// Moving ticket to another department
// Documentation: https://docs.glassix.com/reference/set-department
var (moveDeptSuccess, newTicketId, moveDeptError) = await ticketsClient.SetDepartmentAsync(
    12345,
    departmentId: Guid.Parse("department-guid-here")
);

// Generating PDF of the ticket
// Documentation: https://docs.glassix.com/reference/ticket-pdf
var ticketRenderOptions = new TicketRenderOptions
{
    includeDetails = true,
    includeNotes = true,
    fontSizeInPixels = 14
};

var (pdfSuccess, pdfData, pdfError) = await ticketsClient.GetTicketPdfAsync(
    12345,
    ticketRenderOptions
);

// Generating HTML of the ticket
// Documentation: https://docs.glassix.com/reference/ticket-html
var (htmlSuccess, htmlContent, htmlError) = await ticketsClient.GetTicketHtmlAsync(
    12345,
    ticketRenderOptions
);

// Generating a survey link
// Documentation: https://docs.glassix.com/reference/generate-survey-link
var (surveySuccess, surveyLink, surveyError) = await ticketsClient.GenerateSurveyLinkAsync(
    12345,
    surveyId: 1
);

// Setting a ticket summary
// Documentation: https://docs.glassix.com/reference/post-tickets-setsummary
var (summarySuccess, summaryError) = await ticketsClient.SetTicketSummaryAsync(
    12345,
    summary: "Customer requested a refund for order #38921"
);

// Scrambling (permanently deleting) ticket data
// Documentation: https://docs.glassix.com/reference/scramble-ticket
var (scrambleSuccess, scrambleError) = await ticketsClient.ScrambleTicketAsync(12345);
```

### Working with Users

```csharp
// Get all users
// Documentation: https://docs.glassix.com/reference/get-all-users
var (usersSuccess, users, usersError) = await usersClient.GetAllUsersAsync();

// Set user status
// Documentation: https://docs.glassix.com/reference/set-status
var (setStatusSuccess, setStatusError) = await usersClient.SetUserStatusAsync(User.UserStatus.Online);

// Get user status
// Documentation: https://docs.glassix.com/reference/get-status
var (statusSuccess, status, statusError) = await usersClient.GetUserStatusAsync();

// Get user status logs for a specific time period
// Documentation: https://docs.glassix.com/reference/get-users-statuslogs
var since = DateTime.UtcNow.AddDays(-7);
var until = DateTime.UtcNow;
var (logsSuccess, userStatusLogs, logsError) = await usersClient.GetUserStatusLogsAsync(since, until);

// Get user status logs for a specific user
// Documentation: https://docs.glassix.com/reference/get-users-statuslogs
var userId = Guid.Parse("user-guid-here");
var (userLogsSuccess, userLogs, userLogsError) = await usersClient.GetUserStatusLogsAsync(since, until, userId);

// Update user information
// Documentation: https://docs.glassix.com/reference/user-update
var updateRequest = new UpdateUserRequest
{
    shortName = "John",
    fullName = "John Smith",
    jobTitle = "Senior Support Agent"
};
var (updateSuccess, updateError) = await usersClient.UpdateUserAsync(updateRequest);

// Add new users to the department
// Documentation: https://docs.glassix.com/reference/add-user
var newUsers = new List<AddUserRequest>
{
    new AddUserRequest
    {
        userName = "newagent@example.com",
        uniqueArgument = "agent123"
    },
    new AddUserRequest
    {
        userName = "newbot@example.com",
        uniqueArgument = "bot456"
    }
};
var (addSuccess, addMessage, addError) = await usersClient.AddUsersAsync("AGENT", "SystemUser", newUsers);

// Delete a user from all departments
// Documentation: https://docs.glassix.com/reference/delete-user
var (deleteSuccess, deleteResponse, deleteError) = await usersClient.DeleteUserAsync("agent@example.com");

// Set a unique argument for the current user
// Documentation: https://docs.glassix.com/reference/put_users-setuniqueargument
var (argSuccess, argError) = await usersClient.SetUserUniqueArgumentAsync("agent789");

// Get a user by their unique argument
// Documentation: https://docs.glassix.com/reference/get-by-unique-argument
var (getUserSuccess, user, getUserError) = await usersClient.GetUserByUniqueArgumentAsync("agent789");

// Set roles for a user
// Documentation: https://docs.glassix.com/reference/set-roles
var roles = new List<string> { "SystemUser", "WhatsApp" };
var (rolesSuccess, rolesError) = await usersClient.SetUserRolesAsync("agent@example.com", roles);
```

### Working with Contacts

```csharp
// Get a contact
// Documentation: https://docs.glassix.com/reference/get-contacts
var (contactSuccess, contact, contactError) = await contactsClient.GetContactAsync(Guid.Parse("contact-guid-here"));

// Set contact name
// Documentation: https://docs.glassix.com/reference/set-name
var (nameSuccess, nameResponse, nameError) = await contactsClient.SetContactNameAsync(
    Guid.Parse("contact-guid-here"),
    "New Contact Name"
);

// Add an identifier to a contact
// Documentation: https://docs.glassix.com/reference/add-identifier
var (addIdentifierSuccess, addIdentifierResponse, addIdentifierError) = await contactsClient.AddIdentifierAsync(
    Guid.Parse("contact-guid-here"),
    ContactIdentifier.IdentifierType.PhoneNumber,
    "+15551234567"
);

// Set a unique argument for a contact
// Documentation: https://docs.glassix.com/reference/set-contact-unique-argument
var (uniqueArgSuccess, uniqueArgResponse, uniqueArgError) = await contactsClient.SetUniqueArgumentAsync(
    Guid.Parse("contact-guid-here"),
    "customer-123"
);

// Delete an identifier from a contact
// Documentation: https://docs.glassix.com/reference/delete-identifier
var (deleteIdentifierSuccess, deleteIdentifierResponse, deleteIdentifierError) = await contactsClient.DeleteIdentifierAsync(
    Guid.Parse("contact-guid-here"),
    contactIdentifierId: 1
);
```

### Sending Protocol Messages

```csharp
// Send a message through a protocol (WhatsApp, SMS, etc.)
// Documentation: https://docs.glassix.com/reference/protocols-send
var message = new Message
{
    text = "Hello from GlassixSharp!",
    protocolType = ProtocolType.WhatsApp,
    from = "15551234567",
    to = "15559876543"
};

// The message status will be automatically set to "Pending"
var (success, sentMessage, error) = await protocolsClient.SendProtocolMessageAsync(message);
```

### Working with Webhooks

```csharp
// Get webhook events
// Documentation: https://docs.glassix.com/reference/get-events
var (success, events, error) = await webhooksClient.GetWebhookEventsAsync(deleteEvents: true);

// Delete webhook events
// Documentation: https://docs.glassix.com/reference/delete-event
var (deleteSuccess, deleteError) = await webhooksClient.DeleteWebhookEventsAsync(events);
```

### Working with Tenants

```csharp
// Check if department is online
// Documentation: https://docs.glassix.com/reference/is-online
var (onlineSuccess, isOnline, onlineError) = await tenantsClient.IsOnlineAsync(
    departmentId: Guid.Parse("department-guid"),
    protocolType: ProtocolType.WhatsApp
);

// Get all available tags
// Documentation: https://docs.glassix.com/reference/get-tags
var (tagsSuccess, tags, tagsError) = await tenantsClient.GetTagsAsync();
```

### Working with Canned Replies

```csharp
// Get all canned replies
// Documentation: https://docs.glassix.com/reference/get-all-c-anned-replies
var (success, cannedReplies, error) = await cannedRepliesClient.GetAllCannedRepliesAsync();
```

## Documentation

For complete documentation on the Glassix API, please visit the [Glassix API Reference](https://docs.glassix.com/reference).

## License

This project is licensed under the MIT License - see the LICENSE file for details.