using GlassixSharp.Models;
using GlassixSharp.Webhooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Webhooks
{
    public class WebhooksClient : BaseGlassixClient
    {
        public WebhooksClient(Credentials credentials, Dictionary<string, string>? headers = null) : base(credentials, headers)
        {

        }

        /// <summary>
        /// Gets webhook events
        /// </summary>
        /// <param name="deleteEvents">Whether to delete events after retrieving them</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of webhook events</returns>
        public async Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(bool deleteEvents = true, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<WebhookEvent>>(
                HttpMethod.Get,
                $"{_baseUrl}/webhooks/getevents?deleteEvents={deleteEvents.ToString().ToLower()}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Deletes webhook events specified by receipt handle and message ID
        /// </summary>
        /// <param name="queueReceiptHandle">Queue receipt handle of the webhook event</param>
        /// <param name="queueMessageId">Queue message ID of the webhook event</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> DeleteWebhookEventsAsync(string queueReceiptHandle, string queueMessageId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(queueReceiptHandle))
                throw new ArgumentNullException(nameof(queueReceiptHandle));

            if (string.IsNullOrEmpty(queueMessageId))
                throw new ArgumentNullException(nameof(queueMessageId));

            var requestBody = new
            {
                queueReceiptHandle = queueReceiptHandle,
                queueMessageId = queueMessageId
            };

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/webhooks/deleteevents",
                requestBody,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }
    }
}
