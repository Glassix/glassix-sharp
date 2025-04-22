using GlassixSharp.Models;
using GlassixSharp.Protocols.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Protocols
{
    public class ProtocolsClient : BaseGlassixClient
    {
        public ProtocolsClient(Credentials credentials, Dictionary<string, string>? headers = null) : base(credentials, headers)
        {

        }

        /// <summary>
        /// Sends a message through a protocol (WhatsApp, SMS, etc.)
        /// </summary>
        /// <param name="request">The message to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sent message</returns>
        public async Task<(bool Success, Message Data, string Error)> SendProtocolMessageAsync(Message request, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Message>(
                HttpMethod.Post,
                $"{_baseUrl}/protocols/send",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }
    }
}
