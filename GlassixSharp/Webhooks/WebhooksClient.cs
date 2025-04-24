using GlassixSharp.Models;
using GlassixSharp.Webhooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        public async Task<(bool Success, string Error)> DeleteWebhookEventsAsync(List<WebhookEvent> webhookEvents, CancellationToken cancellationToken = default)
        {
            if(webhookEvents.Count == 0)
            {
                throw new ArgumentException("webhookEvents cannot be empty", nameof(webhookEvents));
            }

            if (webhookEvents.Any(x => string.IsNullOrEmpty(x.queueReceiptHandle) || string.IsNullOrEmpty(x.queueMessageId)))
            {
                throw new ArgumentException("queueReceiptHandle and queueMessageId cannot be null or empty", nameof(webhookEvents));
            }

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/webhooks/deleteevents",
                webhookEvents,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Once your server is configured to receive payloads, it will listen for any delivery that's sent to the endpoint you configured. To ensure that your server only processes webhook deliveries that were sent by Glassix and to ensure that the delivery was not tampered with, you should validate the webhook signature before processing the delivery further.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool IsRequestValid(Dictionary<string, string> headers)
        {
            bool isValid = false;
            // https://docs.glassix.com/docs/validate-outbound-webhooks-and-api#/testing-x-glassix-auth-signature-validation
            string xGlassixAuth = string.Empty;
            string XGlassixAuthDate = string.Empty;

            if (headers.ContainsKey("X-Glassix-Auth"))
            {
                headers.TryGetValue("X-Glassix-Auth", out xGlassixAuth);
            }
            if (headers.ContainsKey("X-Glassix-Auth-Date"))
            {
                headers.TryGetValue("X-Glassix-Auth-Date", out XGlassixAuthDate);
            }

            if (!string.IsNullOrEmpty(xGlassixAuth) && !string.IsNullOrEmpty(XGlassixAuthDate))
            {
                xGlassixAuth = xGlassixAuth.Trim();
                XGlassixAuthDate = XGlassixAuthDate.Trim();
                string sha1 = GenerateSHA1Hash(XGlassixAuthDate, this._credentials.ApiSecret);
                //if(!string.IsNullOrEmpty(xGlassixAuth) && xGlassixAuth.StartsWith("sha1="))
                //{
                //    xGlassixAuth = xGlassixAuth.Substring("sha1=".Length);
                //}
                if (sha1.Equals(xGlassixAuth, StringComparison.OrdinalIgnoreCase))
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        private static string GenerateSHA1Hash(string payload, string secret)
        {
            string hash = string.Empty;
            try
            {
                HMACSHA1 myhmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
                byte[] byteArray = Encoding.UTF8.GetBytes(payload);
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    byte[] hashValue = myhmacsha1.ComputeHash(stream);
                    hash = string.Join("", Array.ConvertAll(hashValue, b => b.ToString("x2")));
                }
                return "sha1=" + hash;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
