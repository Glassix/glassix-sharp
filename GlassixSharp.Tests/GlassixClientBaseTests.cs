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
        protected readonly IGlassixClient? _client;

        protected GlassixClientBaseTests(TestFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
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