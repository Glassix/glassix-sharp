using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Tickets.Models;
using Xunit;
using GlassixSharp.Contacts.Models;

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
            var contactName = $"Test-{DateTime.Now.Ticks % 10000}";
            var emailAddress = $"test{new Random().Next(10, 99)}@{Guid.NewGuid()}.com";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = emailAddress,
                        protocolType = ProtocolType.Mail,
                        subProtocolType = SubProtocolType.MailTo,
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
            Assert.True(result.Data.identifiers.Exists(i => i.identifier == emailAddress));
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
            var contactName = $"Initial-{DateTime.Now.Ticks % 10000}";
            var emailAddress = $"test{new Random().Next(10, 99)}@{Guid.NewGuid()}.com";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = emailAddress,
                        protocolType = ProtocolType.Mail,
                        subProtocolType = SubProtocolType.MailTo,
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

            var newName = $"Updated-{DateTime.Now.Ticks % 10000}";

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

        [Fact]
        public async Task AddIdentifier_ShouldAddIdentifierToContact()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first to get a contact
            var subject = $"Test Ticket for Adding Identifier {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"IdTest-{DateTime.Now.Ticks % 10000}";
            var emailAddress = $"test{new Random().Next(10, 99)}@{Guid.NewGuid()}.com";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = emailAddress,
                        protocolType = ProtocolType.Mail,
                        subProtocolType = SubProtocolType.MailTo,
                        type = Participant.Type.Client
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket for adding contact identifier."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            // Get the contact ID from the created ticket
            var contactId = createResult.Data.participants[0].contactId;
            Assert.NotEqual(Guid.Empty, contactId);

            // Define a new secondary email identifier to add
            var secondaryEmailIdentifier = $"test-secondary-{new Random().Next(1000, 9999)}@example.com";
            
            // Act
            var result = await _contactsClient!.AddIdentifierAsync(
                contactId, 
                ContactIdentifier.IdentifierType.MailAddress, 
                secondaryEmailIdentifier);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data.Message);
            Assert.Null(result.Error);

            // Verify the identifier was added
            var contactResult = await _contactsClient!.GetContactAsync(contactId);
            Assert.True(contactResult.Success);
            Assert.Contains(contactResult.Data.identifiers, i => i.identifier == secondaryEmailIdentifier);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }

        [Fact]
        public async Task SetUniqueArgument_ShouldUpdateContactUniqueArgument()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first to get a contact
            var subject = $"Test Ticket for Unique Argument {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"ArgTest-{DateTime.Now.Ticks % 10000}";
            var emailAddress = $"test{new Random().Next(10, 99)}@{Guid.NewGuid()}.com";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = emailAddress,
                        protocolType = ProtocolType.Mail,
                        subProtocolType = SubProtocolType.MailTo,
                        type = Participant.Type.Client
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket for updating contact unique argument."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            // Get the contact ID from the created ticket
            var contactId = createResult.Data.participants[0].contactId;
            Assert.NotEqual(Guid.Empty, contactId);

            var uniqueArgument = $"test-arg-{new Random().Next(1000, 9999)}";

            // Act
            var result = await _contactsClient!.SetUniqueArgumentAsync(contactId, uniqueArgument);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data.Message);
            Assert.Null(result.Error);

            // Verify the unique argument was updated
            var contactResult = await _contactsClient!.GetContactAsync(contactId);
            Assert.True(contactResult.Success);
            Assert.Equal(uniqueArgument, contactResult.Data.uniqueArgument);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }

        [Fact]
        public async Task DeleteIdentifier_ShouldRemoveIdentifierFromContact()
        {
            SkipIfNotConfigured();

            // Arrange - Create a ticket first to get a contact
            var subject = $"Test Ticket for Deleting Identifier {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var contactName = $"DelTest-{DateTime.Now.Ticks % 10000}";
            var emailAddress = $"test{new Random().Next(10, 99)}@{Guid.NewGuid()}.com";
            
            var createRequest = new CreateTicketRequest
            {
                field1 = subject,
                participants = new List<Participant>
                {
                    new Participant
                    {
                        name = contactName,
                        identifier = emailAddress,
                        protocolType = ProtocolType.Mail,
                        subProtocolType = SubProtocolType.MailTo,
                        type = Participant.Type.Client
                    }
                },
                culture = "en-US",
                field2 = "This is a test ticket for deleting contact identifier."
            };

            var createResult = await _ticketsClient!.CreateTicketAsync(createRequest);
            Assert.True(createResult.Success);

            // Get the contact ID from the created ticket
            var contactId = createResult.Data.participants[0].contactId;
            Assert.NotEqual(Guid.Empty, contactId);
            
            // First add a secondary email identifier
            var secondaryEmailIdentifier = $"test-del-{new Random().Next(1000, 9999)}@example.com";
            var addResult = await _contactsClient!.AddIdentifierAsync(
                contactId, 
                ContactIdentifier.IdentifierType.MailAddress, 
                secondaryEmailIdentifier);
            Assert.True(addResult.Success);
            
            // Get the contact to find the identifier ID
            var contactResult = await _contactsClient!.GetContactAsync(contactId);
            Assert.True(contactResult.Success);
            var addedIdentifier = contactResult.Data.identifiers.Find(i => i.identifier == secondaryEmailIdentifier);
            Assert.NotNull(addedIdentifier);
            
            // Act
            var result = await _contactsClient!.DeleteIdentifierAsync(contactId, addedIdentifier.id);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data.Message);
            Assert.Null(result.Error);

            // Verify the identifier was removed
            var updatedContactResult = await _contactsClient!.GetContactAsync(contactId);
            Assert.True(updatedContactResult.Success);
            Assert.DoesNotContain(updatedContactResult.Data.identifiers, i => i.identifier == secondaryEmailIdentifier);

            // Cleanup
            await _ticketsClient!.SetTicketStateAsync(createResult.Data.id, Ticket.State.Closed);
        }
    }
} 