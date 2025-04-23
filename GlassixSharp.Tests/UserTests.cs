using System;
using System.Linq;
using System.Threading.Tasks;
using GlassixSharp.Users.Models;
using Xunit;

namespace GlassixSharp.Tests
{
    public class UserTests : GlassixClientBaseTests
    {
        public UserTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnUsers()
        {
            SkipIfNotConfigured();

            // Act
            var result = await _usersClient!.GetAllUsersAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task GetUserStatus_ShouldReturnStatus()
        {
            SkipIfNotConfigured();

            // Act
            var result = await _usersClient!.GetUserStatusAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Status);
            Assert.Null(result.Error);
            
            // Status should be one of the known values
            Assert.Contains(result.Data.Status, new[] { User.UserStatus.Online, User.UserStatus.Offline, User.UserStatus.Break, User.UserStatus.Break2, User.UserStatus.Break3, User.UserStatus.Break4, User.UserStatus.Break5 });
        }

        [Theory]
        [InlineData(User.UserStatus.Online)]
        [InlineData(User.UserStatus.Offline)]
        [InlineData(User.UserStatus.Break)]
        public async Task SetUserStatus_ShouldUpdateStatus(User.UserStatus status)
        {
            SkipIfNotConfigured();

            // Act
            var result = await _usersClient!.SetUserStatusAsync(status);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            // Verify the status was updated - allow a small delay for status change to propagate
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            var statusResult = await _usersClient!.GetUserStatusAsync();
            Assert.True(statusResult.Success);

            Assert.Equal(status, statusResult.Data.Status);
        }
    }
} 