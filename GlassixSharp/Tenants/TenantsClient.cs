using GlassixSharp.Models;
using GlassixSharp.Tenants.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Tenants
{
    public class TenantsClient : BaseGlassixClient
    {
        public TenantsClient(Credentials credentials, Dictionary<string, string>? headers = null) : base(credentials, headers)
        {

        }

        /// <summary>
        /// Checks if the department is open at the moment for a specific protocol
        /// </summary>
        /// <param name="departmentId">ID of the department to check</param>
        /// <param name="protocolType">Protocol type to check (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Boolean indicating if the department is online</returns>
        public async Task<(bool Success, bool IsOnline, string Error)> IsOnlineAsync(Guid departmentId, ProtocolType? protocolType = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["departmentId"] = departmentId
            };

            if (protocolType.HasValue)
                queryParams["protocolType"] = protocolType.Value;

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/tenants/isonline";

            if (!string.IsNullOrEmpty(queryString))
                url += $"?{queryString}";

            var response = await SendRequestAsync<bool>(
                HttpMethod.Get,
                url,
                null,
                false, // This endpoint doesn't require authentication as per the API docs
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Gets all available ticket tags from the department
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tag data objects containing names, colors, and parent tags</returns>
        public async Task<(bool Success, List<Tag> tags, string Error)> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<Tag>>(
                HttpMethod.Get,
                $"{_baseUrl}/tenants/gettags",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }
    }
}
