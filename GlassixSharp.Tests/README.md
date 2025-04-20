# GlassixSharp Tests

This project contains integration tests for the GlassixSharp client library. The tests make real API calls to the Glassix API, so you'll need to provide valid credentials.

## Setup

To run the tests, you need to set the following environment variables:

- `GLASSIX_WORKSPACE_NAME` - Your Glassix workspace name
- `GLASSIX_USER_NAME` - Your Glassix username (usually an email)
- `GLASSIX_API_KEY` - Your Glassix API key (as a GUID)
- `GLASSIX_API_SECRET` - Your Glassix API secret

### Setting Environment Variables in PowerShell

```powershell
$env:GLASSIX_WORKSPACE_NAME = "your-workspace-name"
$env:GLASSIX_USER_NAME = "your-username"
$env:GLASSIX_API_KEY = "your-api-key-guid"
$env:GLASSIX_API_SECRET = "your-api-secret"
```

### Setting Environment Variables in Command Prompt

```cmd
set GLASSIX_WORKSPACE_NAME=your-workspace-name
set GLASSIX_USER_NAME=your-username
set GLASSIX_API_KEY=your-api-key-guid
set GLASSIX_API_SECRET=your-api-secret
```

### Setting Environment Variables Permanently (Windows)

1. Open System Properties (Win + Pause/Break or right-click on This PC and select Properties)
2. Click on "Advanced system settings"
3. Click on "Environment Variables"
4. Under "User variables", click "New" and add each variable

## Running the Tests

Once the environment variables are set, you can run the tests using:

```
dotnet test
```

## Test Categories

The tests are organized by API functionality:

- `UserTests` - Tests for user-related operations
- `TicketTests` - Tests for ticket-related operations
- `ContactTests` - Tests for contact-related operations

## Notes

- The tests include automatic cleanup to prevent accumulation of test data. However, you may want to review your Glassix tickets after running tests.
- If the environment variables are not set, the tests will be automatically skipped.
- Each test creates its own test data with unique identifiers to avoid conflicts. 