using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Tickets.Models;
using Xunit;
using System.Linq;
using GlassixSharp.Tenants;

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
            var until = DateTime.UtcNow.AddDays(-1);

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

            // Create and validate a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                protocolType: ProtocolType.Mail
            );

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task GetTicket_ShouldReturnTicket()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first
            var subject = "Test Ticket for Get";
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: subject,
                messageBody: "This is a test ticket for getting by ID.",
                protocolType: ProtocolType.Mail
            );

            // Act
            var result = await _ticketsClient!.GetTicketAsync(ticket.id);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(ticket.id, result.Data.id);
            Assert.Equal(ticket.field1, result.Data.field1);
            Assert.Null(result.Error);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task SendMessage_ShouldSendMessageToTicket()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Message",
                messageBody: "This is a test ticket for sending messages.",
                protocolType: ProtocolType.Mail
            );

            var messageText = $"Test message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var messageRequest = new SendMessageRequest
            {
                text = messageText
            };

            // Act
            var result = await _ticketsClient!.SendMessageAsync(ticket.id, messageRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(messageText, result.Data.text);
            Assert.Null(result.Error);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task AddTicketTags_ShouldAddTags()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Tags",
                messageBody: "This is a test ticket for adding tags.",
                protocolType: ProtocolType.Mail
            );

            var tenantTagsResult = await _tenantsClient!.GetTagsAsync();
            if(tenantTagsResult.tags.Count == 0)
            {
                throw new Exception("No tags available in the tenant. Please create tags before running this test.");
            }

            List<string> tags = new List<string>() { tenantTagsResult.tags.Select(t => t.Name).FirstOrDefault() };

            // Act
            var result = await _ticketsClient!.AddTicketTagsAsync(ticket.id, tags);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Contains(tags.FirstOrDefault(), result.Data);
            Assert.Null(result.Error);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task RemoveTicketTag_ShouldRemoveTag()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Tags Remove",
                messageBody: "This is a test ticket for removing tags.",
                protocolType: ProtocolType.Mail
            );

            // Add tags
            var tenantTagsResult = await _tenantsClient!.GetTagsAsync();
            if (tenantTagsResult.tags.Count == 0)
            {
                throw new Exception("No tags available in the tenant. Please create tags before running this test.");
            }

            string tag = tenantTagsResult.tags.Select(t => t.Name).FirstOrDefault();
            List<string> tags = new List<string>() { tag };

            var addResult = await _ticketsClient!.AddTicketTagsAsync(ticket.id, tags);
            Assert.True(addResult.Success);

            // Act
            var result = await _ticketsClient!.RemoveTicketTagAsync(ticket.id, tag);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.DoesNotContain(tag, result.Data);
            Assert.Null(result.Error);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task SetTicketState_ShouldChangeTicketState()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method with specific initial state
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for State Change",
                messageBody: "This is a test ticket for changing state.",
                protocolType: ProtocolType.Mail
            );

            Ticket.State nextState = Ticket.State.Closed;
            var additionalInfo = new SetTicketStateRequest
            {
                summary = "Ticket snoozed for testing"
            };

            // Act
            var result = await _ticketsClient!.SetTicketStateAsync(ticket.id, nextState, true, true, true, additionalInfo);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Error);

            // Verify state change was applied
            var getResult = await _ticketsClient!.GetTicketAsync(ticket.id);
            Assert.Equal(nextState, getResult.Data.state);
        }

        [Fact]
        public async Task SetTicketFields_ShouldUpdateTicketFields()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket Original",
                protocolType: ProtocolType.Mail
            );

            var updatedSubject = $"Updated Subject {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var fieldsRequest = new SetTicketFieldsRequest
            {
                field1 = updatedSubject,
                field2 = "Updated field 2 content",
                field3 = "New field 3 content",
                uniqueArgument = "test-unique-argument"
            };

            // Act
            var result = await _ticketsClient!.SetTicketFieldsAsync(ticket.id, fieldsRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            // Verify fields were updated
            var getResult = await _ticketsClient!.GetTicketAsync(ticket.id);
            Assert.Equal(updatedSubject, getResult.Data.field1);
            Assert.Equal("Updated field 2 content", getResult.Data.field2);
            Assert.Equal("New field 3 content", getResult.Data.field3);
            Assert.Equal("test-unique-argument", getResult.Data.uniqueArgument);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task SetParticipantName_ShouldUpdateParticipantName()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Participant Update",
                protocolType: ProtocolType.Mail
            );

            var participantId = ticket.participants.First(p => p.type == Participant.Type.Client).id;
            var newName = $"Updated Contact {Guid.NewGuid().ToString().Substring(0, 4)}";

            // Act
            var result = await _ticketsClient!.SetParticipantNameAsync(ticket.id, participantId, newName);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            // Verify participant name was updated
            var getResult = await _ticketsClient!.GetTicketAsync(ticket.id);
            var updatedParticipant = getResult.Data.participants.First(p => p.id == participantId);
            Assert.Equal(newName, updatedParticipant.name);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task AddNote_ShouldAddNoteToTicket()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Notes",
                protocolType: ProtocolType.Mail
            );

            var noteText = $"This is a test note added at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

            // Act
            var result = await _ticketsClient!.AddNoteAsync(ticket.id, noteText);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task SetTicketOwner_ShouldChangeTicketOwner()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket with no auto-assignment
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Owner Change",
                protocolType: ProtocolType.Mail
            );

            var users = await _usersClient!.GetAllUsersAsync();
            string newOwnerUserName = users.Data.FirstOrDefault(u =>
                ticket.owner == null || u.UserName != ticket.owner.UserName)?.UserName;

            // Act
            var result = await _ticketsClient!.SetTicketOwnerAsync(ticket.id, newOwnerUserName);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            // Verify owner was updated
            var getResult = await _ticketsClient!.GetTicketAsync(ticket.id);
            Assert.NotNull(getResult.Data.owner);
            Assert.Equal(newOwnerUserName, getResult.Data.owner.UserName);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task AssignAvailableUser_ShouldAssignTicketToAvailableUser()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket with no auto-assignment
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Auto Assignment",
                protocolType: ProtocolType.Mail
            );

            // Act
            var result = await _ticketsClient!.AssignAvailableUserAsync(ticket.id);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            // Verify ticket has an owner now
            var getResult = await _ticketsClient!.GetTicketAsync(ticket.id);
            Assert.NotNull(getResult.Data.owner);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task SetTicketSummary_ShouldSetSummary()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for Summary",
                protocolType: ProtocolType.Mail
            );

            var summaryText = $"Summary of the test ticket created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

            // Act
            var result = await _ticketsClient!.SetTicketSummaryAsync(ticket.id, summaryText);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            // Verify summary was set
            var getResult = await _ticketsClient!.GetTicketAsync(ticket.id);
            Assert.NotNull(getResult.Data.ticketSummary);
            Assert.Equal(summaryText, getResult.Data.ticketSummary.value);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task GetTicketHtml_ShouldGenerateHtmlDocument()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for HTML Export",
                protocolType: ProtocolType.Mail
            );

            // Add a message to the ticket
            var messageRequest = new SendMessageRequest
            {
                text = $"Test message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
            };
            await _ticketsClient!.SendMessageAsync(ticket.id, messageRequest);

            var renderOptions = new TicketRenderOptions
            {
                includeDetails = true,
                includeConversationLink = true,
                includeNotes = true,
                fontSizeInPixels = 14
            };

            // Act
            var result = await _ticketsClient!.GetTicketHtmlAsync(ticket.id, renderOptions);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.HtmlContent);
            Assert.NotEmpty(result.HtmlContent);
            Assert.Null(result.Error);

            // Verify HTML contains ticket information
            Assert.Contains(ticket.field1, result.HtmlContent);
            Assert.Contains(ticket.participants.First(p => p.type == Participant.Type.Client).name, result.HtmlContent);

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        [Fact]
        public async Task GetTicketPdf_ShouldGeneratePdfDocument()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket using the helper method
            Ticket ticket = await CreateTestTicketAsync(
                subjectPrefix: "Test Ticket for PDF Export",
                protocolType: ProtocolType.Mail
            );

            // Add a message to the ticket
            var messageRequest = new SendMessageRequest
            {
                text = $"Test message sent at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
            };
            await _ticketsClient!.SendMessageAsync(ticket.id, messageRequest);

            var renderOptions = new TicketRenderOptions
            {
                includeDetails = true,
                includeConversationLink = true,
                includeNotes = true,
                fontSizeInPixels = 14,
                replaceContentId = true,
                showParticipantType = true
            };

            // Act
            var result = await _ticketsClient!.GetTicketPdfAsync(ticket.id, renderOptions);
            if(!result.Success)
            {
                throw new Exception($"Failed to generate PDF: {result.Error}");
            }

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.PdfData);
            Assert.True(result.PdfData.Length > 0);
            Assert.Null(result.Error);

            // PDF binary validation - check for PDF signature
            Assert.Equal(0x25, result.PdfData[0]); // %
            Assert.Equal(0x50, result.PdfData[1]); // P
            Assert.Equal(0x44, result.PdfData[2]); // D
            Assert.Equal(0x46, result.PdfData[3]); // F

            // Cleanup
            await CleanupTicketAsync(ticket.id);
        }

        #region Helper Methods

        private async Task<Ticket> CreateTestTicketAsync(string subjectPrefix = "Test Ticket", string messageBody = "This is a test ticket created by automated tests.", ProtocolType protocolType = ProtocolType.Mail)
        {
            bool assignAvailableUser = true;

            // Create unique identifiers
            var subject = $"{subjectPrefix} {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"Test Contact {Guid.NewGuid()}";
            string identifier = string.Empty;
            SubProtocolType subProtocolType = SubProtocolType.Undefined;

            // Set up protocol-specific details
            switch(protocolType)
            {
                case ProtocolType.Mail:
                    identifier = $"test{new Random().Next(10, 99)}@{Guid.NewGuid()}.com";
                    subProtocolType = SubProtocolType.MailTo;
                    break;
                case ProtocolType.WhatsApp:
                    identifier = $"1234567890{new Random().Next(10, 99)}";
                    break;
            }

            // Create the request
            var request = new CreateTicketRequest
            {
                field1 = subject, // Subject is stored in field1
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = identifier,
                        protocolType = protocolType,
                        type = Participant.Type.Client,
                        subProtocolType = subProtocolType
                    }
                },
                culture = "en-US",
                field2 = messageBody, // Message in field2
                getAvailableUser = assignAvailableUser
            };

            // Create the ticket
            var result = await _ticketsClient!.CreateTicketAsync(request);
            if(!result.Success)
            {
                throw new Exception($"Failed to create ticket: {result.Error}");
            }
            Assert.NotNull(result.Data);
            Assert.True(result.Data.id > 0);
            Assert.Equal(subject, result.Data.field1);
            Assert.Null(result.Error);

            return result.Data;
        }

        private async Task CleanupTicketAsync(int ticketId)
        {
            if (ticketId > 0)
            {
                var result = await _ticketsClient!.SetTicketStateAsync(ticketId, Ticket.State.Closed);
                if (!result.Success)
                {
                    throw new Exception($"Failed to close ticket: {result.Error}");
                }
            }
        }

        #endregion
    }
}