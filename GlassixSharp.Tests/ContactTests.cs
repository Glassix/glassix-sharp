using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Tickets.Models;
using Xunit;

namespace GlassixSharp.Tests
{
    public class ContactTests : GlassixClientBaseTests
    {
        public ContactTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetContact_ShouldReturnContact()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first to get a contact
            var subject = $"Test Ticket for Contact {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
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
                field2 = "This is a test ticket for retrieving contact."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            // Get the contact ID from the created ticket
            var contactId = createResult.Data.participants[0].contactId;
            Assert.NotEqual(Guid.Empty, contactId);

            // Act
            var result = await _contactsClient!.GetContactAsync(contactId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(contactId, result.Data.id);
            Assert.Contains(contactName, result.Data.name);
            Assert.True(result.Data.identifiers.Exists(i => i.identifier == phoneNumber));
            Assert.Null(result.Error);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }

        [Fact]
        public async Task SetContactName_ShouldUpdateContactName()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first to get a contact
            var subject = $"Test Ticket for Contact Name {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"Initial Contact {Guid.NewGuid()}";
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
                field2 = "This is a test ticket for updating contact name."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            // Get the contact ID from the created ticket
            var contactId = createResult.Data.participants[0].contactId;
            Assert.NotEqual(Guid.Empty, contactId);

            var newName = $"Updated Contact {Guid.NewGuid()}";

            // Act
            var result = await _contactsClient!.SetContactNameAsync(contactId, newName);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data.Message);
            Assert.Null(result.Error);

            // Verify the name was updated
            var contactResult = await _contactsClient!.GetContactAsync(contactId);
            Assert.True(contactResult.Success);
            Assert.Equal(newName, contactResult.Data.name);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }
    }
} 