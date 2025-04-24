using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Webhooks.Models;
using Xunit;
using System.Linq;
using System.Text.Json;

namespace GlassixSharp.Tests
{
    public class WebhooksTests : GlassixClientBaseTests
    {
        public WebhooksTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task DeleteWebhookEvents_ShouldDeleteEvent()
        {
            SkipIfNotConfigured();

            // Arrange - Get events first
            var getResult = await _webhooksClient!.GetWebhookEventsAsync(false);
            Assert.True(getResult.Success);

            if (getResult.Data.Count == 0)
            {
                // Skip test if no events are available
                return;
            }

            // Get the first event's queue info
            var webhookEvent = getResult.Data.First();
            Assert.NotNull(webhookEvent.queueReceiptHandle);
            Assert.NotNull(webhookEvent.queueMessageId);

            // Act
            var result = await _webhooksClient!.DeleteWebhookEventsAsync(new List<WebhookEvent>() { webhookEvent });

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        [Fact]
        public void IsRequestValid_WithValidHeaders_ShouldReturnTrue()
        {
            SkipIfNotConfigured();

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "X-Glassix-Auth", Environment.GetEnvironmentVariable("WEBHOOKS_HEADER_X_GLASSIX_AUTH") },
                { "X-Glassix-Auth-Date", Environment.GetEnvironmentVariable("WEBHOOKS_HEADER_X_GLASSIX_AUTH_DATE") },
                { "X-Glassix-Api-Key", Environment.GetEnvironmentVariable("API_KEY")?.ToLower() },
                { "X-Glassix-Request-Count", "1" }
            };

            // Create a mock of the expected hash with our test API Secret
            // Note: For the test to pass, the hash must match what would be generated
            // with the API Secret used in the test configuration

            // Act
            bool isValid = _webhooksClient!.IsRequestValid(headers);

            // Assert
            Assert.True(isValid);
        }
    }
} 