using GlassixSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlassixSharp.Tests
{
    public class TenantsTests : GlassixClientBaseTests
    {
        public TenantsTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetTags_ShouldReturnAvailableTags()
        {
            SkipIfNotConfigured();

            // Act
            var result = await _tenantsClient!.GetTagsAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.tags);
            Assert.IsType<List<GlassixSharp.Tenants.Models.Tag>>(result.tags);
            Assert.NotEmpty(result.tags);
            Assert.True(result.tags.Count > 1, "Expected to get more than one tag from the system");
        }

        [Fact]
        public async Task IsOnline_ShouldReturnDepartmentOnlineStatus()
        {
            SkipIfNotConfigured();

            // Arrange - Get department ID from environment variable
            var departmentIdString = Environment.GetEnvironmentVariable("API_KEY");
            Assert.NotNull(departmentIdString);
            Assert.True(Guid.TryParse(departmentIdString, out Guid departmentId));
            Assert.NotEqual(Guid.Empty, departmentId);

            // Act - Check if the department is online
            var result = await _tenantsClient!.IsOnlineAsync(departmentId);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task IsOnline_WithProtocolType_ShouldReturnDepartmentOnlineStatusForProtocol()
        {
            SkipIfNotConfigured();

            // Arrange - Get department ID from environment variable
            var departmentIdString = Environment.GetEnvironmentVariable("API_KEY");
            Assert.NotNull(departmentIdString);
            Assert.True(Guid.TryParse(departmentIdString, out Guid departmentId));
            Assert.NotEqual(Guid.Empty, departmentId);
            
            // Act - Check if the department is online for Mail protocol
            var result = await _tenantsClient!.IsOnlineAsync(departmentId, ProtocolType.Mail);

            // Assert
            Assert.True(result.Success);
        }
    }
}
