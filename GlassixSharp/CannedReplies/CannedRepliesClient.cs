using GlassixSharp.CannedReplies.Models;
using GlassixSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.CannedReplies
{
    class CannedRepliesClient : BaseGlassixClient
    {
        public CannedRepliesClient(Credentials credentials, Dictionary<string, string>? headers = null) : base(credentials, headers)
        {

        }

        /// <summary>
        /// Gets all canned replies available in the department
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of canned replies</returns>
        public async Task<(bool Success, List<CannedReply> cannedReplies, string Error)> GetAllCannedRepliesAsync(CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<CannedReply>>(
                HttpMethod.Get,
                $"{_baseUrl}/cannedreplies/getall",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }
    }
}
