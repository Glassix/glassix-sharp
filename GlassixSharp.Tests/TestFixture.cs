using System;
using GlassixSharp;
using GlassixSharp.Contacts;
using GlassixSharp.Models;
using GlassixSharp.Protocols;
using GlassixSharp.Tickets;
using GlassixSharp.Users;
using GlassixSharp.Webhooks;

namespace GlassixSharp.Tests
{
    public class TestFixture : IDisposable
    {
        public ContactsClient? contactsClient { get; private set; }
        public ProtocolsClient? protocolsClient { get; private set; }
        public TicketsClient? ticketsClient { get; private set; }
        public UsersClient? usersClient { get; private set; }
        public WebhooksClient? webhooksClient { get; private set; }

        public TestFixture()
        {
            // Load credentials from environment variables
            string? workspaceName = Environment.GetEnvironmentVariable("WORKSPACE_NAME");
            string? userName = Environment.GetEnvironmentVariable("USER_NAME");
            string? apiKeyString = Environment.GetEnvironmentVariable("API_KEY");
            string? apiSecret = Environment.GetEnvironmentVariable("API_SECRET");

            // Skip initialization if any credentials are missing
            if (string.IsNullOrEmpty(workspaceName) ||
                string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(apiKeyString) ||
                string.IsNullOrEmpty(apiSecret) ||
                !Guid.TryParse(apiKeyString, out Guid apiKey))
            {
                throw new Exception("Glassix credentials not found in environment variables. Tests will be skipped.");
            }

            // Create credentials
            var credentials = new Credentials(
                workspaceName,
                userName,
                apiKey,
                apiSecret,
                timeoutSeconds: 30,
                isTestingEnvironment: true
            );

            // Initialize clients
            contactsClient = new ContactsClient(credentials);
            protocolsClient = new ProtocolsClient(credentials);
            ticketsClient = new TicketsClient(credentials);
            usersClient = new UsersClient(credentials);
            webhooksClient = new WebhooksClient(credentials);

            this.IsConfigured = true;
        }

        public void Dispose()
        {
            // Cleanup code if needed
        }

        /// <summary>
        /// Checks if the client is configured with valid credentials
        /// </summary>
        public bool IsConfigured = false;
    }
} 