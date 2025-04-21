using System;
using GlassixSharp;
using GlassixSharp.Models;

namespace GlassixSharp.Tests
{
    public class TestFixture : IDisposable
    {
        public IGlassixClient? Client { get; private set; }

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

            // Initialize client
            Client = new GlassixClient(credentials);
        }

        public void Dispose()
        {
            // Cleanup code if needed
        }

        /// <summary>
        /// Checks if the client is configured with valid credentials
        /// </summary>
        public bool IsConfigured => Client != null;
    }
} 