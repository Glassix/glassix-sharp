using GlassixSharp.Models;
using GlassixSharp.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
