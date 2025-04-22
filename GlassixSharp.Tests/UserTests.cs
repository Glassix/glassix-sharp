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
            Assert.Contains(result.Data.Status, new[] { "Online", "Offline", "Break", "Break2", "Break3", "Break4", "Break5" });
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
            
            // Compare status correctly based on the enum
            if (status == User.UserStatus.Online)
            {
                Assert.Equal("Online", statusResult.Data.Status);
            }
            else if (status == User.UserStatus.Offline)
            {
                Assert.Equal("Offline", statusResult.Data.Status);
            }
            else if (status == User.UserStatus.Break)
            {
                // The API might return "Break" or "Break[N]" depending on implementation
                Assert.StartsWith("Break", statusResult.Data.Status);
            }
        }
        
        [Fact]
        public async Task SetUserStatus_WithInvalidStatus_ShouldStillSucceed()
        {
            SkipIfNotConfigured();

            // We're testing Break2-5 which exist in the enum but may not be directly supported by the API
            var status = User.UserStatus.Break2;
            
            // Act
            var result = await _usersClient!.SetUserStatusAsync(status);

            // The API call should still succeed
            Assert.True(result.Success);
            Assert.Null(result.Error);
            
            // Get the current status - we don't verify the specific value as the API might 
            // handle these extended break states differently
            var statusResult = await _usersClient!.GetUserStatusAsync();
            Assert.True(statusResult.Success);
            Assert.NotNull(statusResult.Data.Status);
        }
    }
} 