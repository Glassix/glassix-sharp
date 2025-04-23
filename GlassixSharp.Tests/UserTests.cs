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

        [Fact]
        public async Task GetUserStatusLogs_ShouldReturnStatusLogs()
        {
            SkipIfNotConfigured();

            // Arrange
            var since = DateTime.Now.AddDays(-7);
            var until = DateTime.Now;

            // Act
            var result = await _usersClient!.GetUserStatusLogsAsync(since, until);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task GetUserStatusLogsWithUserId_ShouldReturnStatusLogsForSpecificUser()
        {
            SkipIfNotConfigured();

            // Arrange
            var since = DateTime.Now.AddDays(-7);
            var until = DateTime.Now;

            // Get a user ID from the system
            var usersResult = await _usersClient!.GetAllUsersAsync();
            Assert.True(usersResult.Success);
            Assert.NotEmpty(usersResult.Data);

            var userId = usersResult.Data.First().id;

            // Act
            var result = await _usersClient!.GetUserStatusLogsAsync(since, until, userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateUserDetails()
        {
            SkipIfNotConfigured();

            // Arrange
            var request = new UpdateUserRequest
            {
                shortName = "Test Short Name",
                fullName = "Test Full Name",
                jobTitle = "Test Job Title"
            };

            // Act
            var result = await _usersClient!.UpdateUserAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task AddUsers_ShouldAddUsersToSystem()
        {
            SkipIfNotConfigured();

            // Arrange
            string userType = "AGENT";
            var role = "SystemUser";
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);

            var users = new List<AddUserRequest>
            {
                new AddUserRequest
                {
                    userName = $"testuser_{uniqueId}@example.com",
                    uniqueArgument = $"test-arg-{uniqueId}"
                }
            };

            // Act
            var result = await _usersClient!.AddUsersAsync(userType, role, users);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Message);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task SetUserUniqueArgument_ShouldUpdateUniqueArgument()
        {
            SkipIfNotConfigured();

            // Arrange
            var uniqueArgument = $"unique-arg-{Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            var result = await _usersClient!.SetUserUniqueArgumentAsync(uniqueArgument);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task GetUserByUniqueArgument_ShouldReturnUser()
        {
            SkipIfNotConfigured();

            // Arrange
            // First set a unique argument
            var uniqueArgument = $"unique-arg-{Guid.NewGuid().ToString().Substring(0, 8)}";
            var setResult = await _usersClient!.SetUserUniqueArgumentAsync(uniqueArgument);
            Assert.True(setResult.Success);

            // Allow some time for the change to propagate
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Act
            var result = await _usersClient!.GetUserByUniqueArgumentAsync(uniqueArgument);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(uniqueArgument, result.Data.uniqueArgument);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task DeleteUser_ShouldDeleteUser()
        {
            SkipIfNotConfigured();

            // Arrange
            // First create a test user
            string userType = "AGENT";
            var role = "SystemUser";
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var userName = $"testdelete_{uniqueId}@example.com";

            var users = new List<AddUserRequest>
            {
                new AddUserRequest
                {
                    userName = userName,
                    uniqueArgument = $"test-delete-{uniqueId}"
                }
            };

            var addResult = await _usersClient!.AddUsersAsync(userType, role, users);
            Assert.True(addResult.Success);

            // Allow some time for the user to be created
            await Task.Delay(TimeSpan.FromSeconds(2));

            // Act
            var result = await _usersClient!.DeleteUserAsync(userName);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.DeletedFromDepartments);
            Assert.Null(result.Error);
        }
    }
} 