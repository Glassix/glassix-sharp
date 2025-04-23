using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Protocols.Models;
using Xunit;

namespace GlassixSharp.Tests
{
    public class ProtocolsTests : GlassixClientBaseTests
    {
        public ProtocolsTests(TestFixture fixture) : base(fixture)
        {
        }
        
        [Fact]
        public async Task SendProtocolMessage_SMS_Primary_ShouldReturnSuccess()
        {
            SkipIfNotConfigured();
            
            // Arrange
            var message = new Message
            {
                text = $"Test SMS message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                protocolType = ProtocolType.SMS,
                from = "972526264195",
                to = "972548495037",
                dateTime = DateTime.UtcNow,
                attachmentUris = new List<string>(),
                files = new List<string>()
            };
            
            // Act
            var result = await _protocolsClient!.SendProtocolMessageAsync(message);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success, "Expected to send SMS message successfully");
            // Note: In a test environment, this might not actually succeed without valid recipients
            // So we're just verifying the method executes without throwing
        }
    }
}
