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
    }
} 