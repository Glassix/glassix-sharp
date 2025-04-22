using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Tickets.Models;
using Xunit;

namespace GlassixSharp.Tests
{
    public class TicketTests : GlassixClientBaseTests
    {
        public TicketTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ListTickets_ShouldReturnTickets()
        {
            SkipIfNotConfigured();

            // Arrange
            var since = DateTime.UtcNow.AddDays(-7);
            var until = DateTime.UtcNow;

            // Act
            var result = await _ticketsClient!.ListTicketsAsync(since, until);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task CreateTicket_ShouldCreateNewTicket()
        {
            SkipIfNotConfigured();

            // Arrange
            var subject = $"Test Ticket {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"Test Contact {Guid.NewGuid()}";
            var phoneNumber = $"+1234567890{new Random().Next(10, 99)}";
            
            var request = new CreateTicketRequest
            {
                field1 = subject, // Subject is stored in field1
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = phoneNumber,
                        protocolType = ProtocolType.WhatsApp,
                        type = Participant.Type.Client
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket created by automated tests." // Message in field2
            };

            // Act
            var result = await _ticketsClient!.CreateTicketAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.id > 0);
            Assert.Equal(subject, result.Data.field1);
            Assert.Null(result.Error);

            // Cleanup - move ticket to closed state
            if (result.Data.id > 0)
            {
                await _ticketsClient!.SetTicketStateAsync(result.Data.id, Ticket.State.Closed);
            }
        }

        [Fact]
        public async Task GetTicket_ShouldReturnTicket()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first
            string subject = $"Test Ticket for Get {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}";
            string contactName = $"Test Contact {Guid.NewGuid()}";
            string identifier = $"abc{new Random().Next(10, 99)}@{Guid.NewGuid()}.com";

            CreateTicketRequest createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = identifier,
                        protocolType = ProtocolType.Mail,
                        type = Participant.Type.Client,
                        subProtocolType = SubProtocolType.MailTo
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket for getting by ID."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            // Act
            var result = await _ticketsClient!.GetTicketAsync(createResult.Data.id);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(createResult.Data.id, result.Data.id);
            Assert.Equal(subject, result.Data.field1);
            Assert.Null(result.Error);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(result.Data.id, Ticket.State.Closed);
        }

        [Fact]
        public async Task SendMessage_ShouldSendMessageToTicket()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first
            var subject = $"Test Ticket for Message {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"Test Contact {Guid.NewGuid()}";
            var phoneNumber = $"+1234567890{new Random().Next(10, 99)}";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = phoneNumber,
                        protocolType = ProtocolType.WhatsApp,
                        type = Participant.Type.Client
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket for sending messages."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            var messageText = $"Test message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var messageRequest = new SendMessageRequest
            {
                text = messageText
            };

            // Act
            var result = await _ticketsClient!.SendMessageAsync(createResult.Data.id, messageRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(messageText, result.Data.text);
            Assert.Null(result.Error);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }

        [Fact]
        public async Task AddTicketTags_ShouldAddTags()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first
            var subject = $"Test Ticket for Tags {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"Test Contact {Guid.NewGuid()}";
            var phoneNumber = $"+1234567890{new Random().Next(10, 99)}";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = phoneNumber,
                        protocolType = ProtocolType.WhatsApp,
                        type = Participant.Type.Client
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket for adding tags."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            var tags = new List<string> { "test-tag", "automated-test" };

            // Act
            var result = await _ticketsClient!.AddTicketTagsAsync(createResult.Data.id, tags);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Contains("test-tag", result.Data);
            Assert.Contains("automated-test", result.Data);
            Assert.Null(result.Error);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }

        [Fact]
        public async Task RemoveTicketTag_ShouldRemoveTag()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first
            var subject = $"Test Ticket for Tags Remove {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"Test Contact {Guid.NewGuid()}";
            var phoneNumber = $"+1234567890{new Random().Next(10, 99)}";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = phoneNumber,
                        protocolType = ProtocolType.WhatsApp,
                        type = Participant.Type.Client
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket for removing tags."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            // Add tags
            var tags = new List<string> { "test-tag", "automated-test" };
            var addResult = await _ticketsClient!.AddTicketTagsAsync(createResult.Data.id, tags);
            Assert.True(addResult.Success);

            // Act
            var result = await _ticketsClient!.RemoveTicketTagAsync(createResult.Data.id, "test-tag");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.DoesNotContain("test-tag", result.Data);
            Assert.Contains("automated-test", result.Data);
            Assert.Null(result.Error);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }
    }
} 