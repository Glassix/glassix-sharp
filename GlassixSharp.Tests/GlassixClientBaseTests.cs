using GlassixSharp.Contacts;
using GlassixSharp.Protocols;
using GlassixSharp.Tickets;
using GlassixSharp.Users;
using GlassixSharp.Webhooks;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GlassixSharp.Tests
{
    public abstract class GlassixClientBaseTests : IClassFixture<TestFixture>
    {
        protected readonly TestFixture _fixture;
        protected readonly ContactsClient? _contactsClient;
        protected readonly ProtocolsClient? _protocolsClient;
        protected readonly TicketsClient? _ticketsClient;
        protected readonly UsersClient? _usersClient;
        protected readonly WebhooksClient? _webhooksClient;

        protected GlassixClientBaseTests(TestFixture fixture)
        {
            _fixture = fixture;
            _contactsClient = fixture.contactsClient;
            _protocolsClient = fixture.protocolsClient;
            _ticketsClient = fixture.ticketsClient;
            _usersClient = fixture.usersClient;
            _webhooksClient = fixture.webhooksClient;
        }

        protected void SkipIfNotConfigured()
        {
            if (!_fixture.IsConfigured)
            {
                throw new Xunit.SkipException("Tests skipped because Glassix credentials are not configured");
            }
        }
    }
}