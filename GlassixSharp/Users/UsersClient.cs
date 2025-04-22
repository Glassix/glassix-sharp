using GlassixSharp.Models;
using GlassixSharp.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlassixSharp.Users.Models.UserInfoData.UserDateData;

namespace GlassixSharp.Users
{
    public class UsersClient : BaseGlassixClient
    {
        public UsersClient(Credentials credentials, Dictionary<string, string>? headers = null) : base(credentials, headers)
        {

        }

        /// <summary>
        /// Gets all users in the department
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of users</returns>
        public async Task<(bool Success, List<User> Data, string Error)> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<User>>(
                HttpMethod.Get,
                $"{_baseUrl}/users/allusers",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets the status of the current user
        /// </summary>
        /// <param name="nextStatus">The new status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetUserStatusAsync(User.UserStatus nextStatus, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/users/setstatus",
                new { nextStatus = nextStatus.ToString() },
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Gets the status of the current user
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The user's status</returns>
        public async Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<UserStatusResponse>(
                HttpMethod.Get,
                $"{_baseUrl}/users/getstatus",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Gets status logs for users within a specific time frame
        /// </summary>
        /// <param name="since">Start date for the query</param>
        /// <param name="until">End date for the query</param>
        /// <param name="userId">Optional specific user ID to get logs for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of user status logs</returns>
        public async Task<(bool Success, List<UserStatusLog> Data, string Error)> GetUserStatusLogsAsync(DateTime since, DateTime until, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["since"] = since.ToString("dd/MM/yyyy"),
                ["until"] = until.ToString("dd/MM/yyyy")
            };

            if (userId.HasValue)
                queryParams["userId"] = userId.Value.ToString();

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/users/statuslogs";

            if (!string.IsNullOrEmpty(queryString))
                url += $"?{queryString}";

            var response = await SendRequestAsync<List<UserStatusLog>>(
                HttpMethod.Get,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Updates information for the current user
        /// </summary>
        /// <param name="request">The update request with user information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/users/update",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Adds new users to the current department
        /// </summary>
        /// <param name="userType">The type of user (AGENT, BOT, API)</param>
        /// <param name="role">The role of the user (SystemUser, ReadOnly)</param>
        /// <param name="users">List of users to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation with message</returns>
        public async Task<(bool Success, string Message, string Error)> AddUsersAsync(User.Type userType, string role, List<AddUserRequest> users, CancellationToken cancellationToken = default)
        {
            if(role != "SystemUser" && role != "ReadOnly")
            {
                throw new ArgumentException("Role must be either 'SystemUser' or 'ReadOnly'.", nameof(role));
            }

            if (userType == User.Type.UNDEFINED)
            {
                throw new ArgumentException("Invalid userType");
            }

            var queryParams = new Dictionary<string, object>
            {
                ["userType"] = userType.ToString(),
                ["role"] = role
            };

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/users/add?{queryString}";

            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Post,
                url,
                users,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data?.Message, response.ErrorMessage);
        }

        /// <summary>
        /// Deletes a user from all departments
        /// </summary>
        /// <param name="userName">Email address of the user to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation with deleted departments</returns>
        public async Task<(bool Success, DeleteUserResponse Data, string Error)> DeleteUserAsync(string userName, CancellationToken cancellationToken = default)
        {
            string url = $"{_baseUrl}/users/delete?UserName={Uri.EscapeDataString(userName)}";

            var response = await SendRequestAsync<DeleteUserResponse>(
                HttpMethod.Delete,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets a unique argument for the current user in this department
        /// </summary>
        /// <param name="nextUniqueArgument">The new unique argument</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetUserUniqueArgumentAsync(string nextUniqueArgument, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/users/setuniqueargument",
                new { nextUniqueArgument },
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Gets a user by their unique argument
        /// </summary>
        /// <param name="uniqueArgument">The unique argument to search for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The user matching the unique argument</returns>
        public async Task<(bool Success, User Data, string Error)> GetUserByUniqueArgumentAsync(string uniqueArgument, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(uniqueArgument))
            {
                throw new ArgumentException("Unique argument cannot be null or empty.", nameof(uniqueArgument));
            }

            string url = $"{_baseUrl}/users/getbyuniqueargument?uniqueArgument={Uri.EscapeDataString(uniqueArgument)}";

            var response = await SendRequestAsync<User>(
                HttpMethod.Get,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets roles for a user
        /// </summary>
        /// <param name="userName">Email address of the user</param>
        /// <param name="roles">List of roles to assign</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetUserRolesAsync(string userName, List<string> roles, CancellationToken cancellationToken = default)
        {
            string url = $"{_baseUrl}/users/setroles?userName={Uri.EscapeDataString(userName)}";

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                url,
                roles,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }
    }
}
