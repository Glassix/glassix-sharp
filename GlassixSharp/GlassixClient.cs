using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Models.Requests;
using GlassixSharp.Models.Responses;

namespace GlassixSharp
{
    /// <summary>
    /// Client for interacting with the Glassix API
    /// </summary>
    public class GlassixClient : IGlassixClient
    {
        private static readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly Credentials _credentials;

        private readonly Dictionary<string, string> _customHeaders = new Dictionary<string, string>();
        private static readonly ConcurrentDictionary<string, (string Token, DateTime ExpiresAt)> _tokens = new ConcurrentDictionary<string, (string, DateTime)>();
        private static readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);

        static GlassixClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Creates a new instance of the GlassixSharp client
        /// </summary>
        /// <param name="credentials">The credentials to use for authentication</param>
        /// <param name="headers">Custom headers that will be sent on every request</param>
        public GlassixClient(Credentials credentials, Dictionary<string, string>? headers = null)
        {
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));

            string glassixDomain = "glassix.com";
            if (credentials.IsTestingEnvironment)
            {
                glassixDomain = "glassix-dev.com";
            }
            _baseUrl = $"https://{_credentials.WorkspaceName}.{glassixDomain}/api/v1.2";

            if (_credentials.TimeoutSeconds > 0)
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(_credentials.TimeoutSeconds);
            }

            if (headers != null)
            {
                _customHeaders = headers;
            }
        }

        /// <summary>
        /// Gets an access token for the Glassix API
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The access token</returns>
        private async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            string tokenKey = $"{_credentials.WorkspaceName}:{_credentials.ApiKey}:{_credentials.UserName}";

            if (_tokens.TryGetValue(tokenKey, out var tokenInfo) && DateTime.UtcNow < tokenInfo.ExpiresAt.AddMinutes(-5))
                return tokenInfo.Token;

            try
            {
                await _tokenSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                // Double-check after acquiring the semaphore
                if (_tokens.TryGetValue(tokenKey, out tokenInfo) && DateTime.UtcNow < tokenInfo.ExpiresAt.AddMinutes(-5))
                    return tokenInfo.Token;

                TokenRequest request = new TokenRequest
                {
                    apiKey = _credentials.ApiKey.ToString(),
                    apiSecret = _credentials.ApiSecret,
                    userName = _credentials.UserName
                };

                var response = await SendRequestAsync<TokenResponse>(
                    HttpMethod.Post,
                    $"{_baseUrl}/token/get",
                    request,
                    false,
                    cancellationToken).ConfigureAwait(false);

                if (response.IsSuccess)
                {
                    if(string.IsNullOrEmpty(response.Data.access_token))
                        throw new Exception("Access token is empty");

                    DateTime expiresAt = DateTime.UtcNow.AddSeconds(response.Data.expires_in);
                    _tokens[tokenKey] = (response.Data.access_token, expiresAt);
                    return response.Data.access_token;
                }

                throw new Exception($"Failed to obtain token: {response.ErrorMessage}");
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }

        JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        /// <summary>
        /// Sends a request to the Glassix API
        /// </summary>
        /// <typeparam name="T">The expected response type</typeparam>
        /// <param name="method">HTTP method</param>
        /// <param name="url">URL to send the request to</param>
        /// <param name="body">Request body (optional)</param>
        /// <param name="requiresAuth">Whether the request requires authentication</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>API response</returns>
        private async Task<ApiResponse<T>> SendRequestAsync<T>(
            HttpMethod method,
            string url,
            object body = null,
            bool requiresAuth = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method, url);

                if (requiresAuth)
                {
                    string token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // Add custom headers
                if(_customHeaders != null && _customHeaders.Count > 0)
                {
                    foreach (var header in _customHeaders)
                    {
                        if(!string.IsNullOrEmpty(header.Key) && !string.IsNullOrEmpty(header.Value))
                        {
                            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                }

                if (body != null)
                {
                    string json = JsonSerializer.Serialize(body, _jsonSerializerOptions);

                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(EmptyResponse))
                    {
                        return ApiResponse<T>.Success((T)(object)new EmptyResponse());
                    }

                    var result = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
                    return ApiResponse<T>.Success(result);
                }

                string errorMessage = response.ReasonPhrase;
                if(!string.IsNullOrEmpty(content))
                {
                    errorMessage = content;
                }

                return ApiResponse<T>.Error(errorMessage, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error(ex.Message);
            }
        }

        /// <summary>
        /// Builds a query string from a dictionary of parameters
        /// </summary>
        /// <param name="parameters">Dictionary of parameters</param>
        /// <returns>Query string</returns>
        private static string BuildQueryString(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return string.Empty;

            var queryParams = new List<string>();

            foreach (var param in parameters)
            {
                if (param.Value == null)
                    continue;

                string stringValue;

                if (param.Value is DateTime dateTime)
                {
                    stringValue = dateTime.ToString("dd/MM/yyyy HH:mm:ss:ff");
                }
                else if (param.Value is Enum)
                {
                    stringValue = param.Value.ToString();
                }
                else if (param.Value is bool boolValue)
                {
                    stringValue = boolValue.ToString().ToLower();
                }
                else
                {
                    stringValue = param.Value.ToString();
                }

                if (!string.IsNullOrEmpty(stringValue))
                {
                    queryParams.Add($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(stringValue)}");
                }
            }

            return string.Join("&", queryParams);
        }

        #region API Methods

        // Tickets

        /// <summary>
        /// Creates a new ticket
        /// </summary>
        /// <param name="request">The ticket creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created ticket</returns>
        public async Task<(bool Success, Ticket Data, string Error)> CreateTicketAsync(
            CreateTicketRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Ticket>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/create",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Gets a ticket by ID
        /// </summary>
        /// <param name="ticketId">ID of the ticket to get</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested ticket</returns>
        public async Task<(bool Success, Ticket Data, string Error)> GetTicketAsync(
            int ticketId,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Ticket>(
                HttpMethod.Get,
                $"{_baseUrl}/tickets/get/{ticketId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Lists tickets within a specific time frame
        /// </summary>
        /// <param name="since">Start date/time for the query</param>
        /// <param name="until">End date/time for the query</param>
        /// <param name="ticketState">Filter by ticket state (optional)</param>
        /// <param name="sortOrder">Sort order (optional)</param>
        /// <param name="page">Page token for pagination (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tickets matching the criteria</returns>
        public async Task<(bool Success, TicketListResponse Data, string Error)> ListTicketsAsync(
            DateTime since,
            DateTime until,
            Ticket.State? ticketState = null,
            SortOrder? sortOrder = null,
            string page = null,
            CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["since"] = since,
                ["until"] = until
            };

            if (ticketState.HasValue)
                queryParams["ticketState"] = ticketState.Value;

            if (sortOrder.HasValue)
                queryParams["sortOrder"] = sortOrder.Value;

            if (!string.IsNullOrEmpty(page))
                queryParams["page"] = page;

            var queryString = BuildQueryString(queryParams);
            var url = $"{_baseUrl}/tickets/list";

            if (!string.IsNullOrEmpty(queryString))
                url += $"?{queryString}";

            var response = await SendRequestAsync<TicketListResponse>(
                HttpMethod.Get,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sends a message in a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="request">The message to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The message transaction</returns>
        public async Task<(bool Success, Transaction Data, string Error)> SendMessageAsync(
            int ticketId,
            SendMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Transaction>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/send/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets the state of a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="nextState">The new state for the ticket</param>
        /// <param name="getTicket">Whether to return the updated ticket</param>
        /// <param name="sendTicketStateChangedMessage">Whether to send a message about the state change</param>
        /// <param name="enableWebhook">Whether to trigger webhooks for this change</param>
        /// <param name="body">Additional parameters (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, MessageResponse Data, string Error)> SetTicketStateAsync(
            int ticketId,
            Ticket.State nextState,
            bool getTicket = false,
            bool sendTicketStateChangedMessage = true,
            bool enableWebhook = true,
            SetTicketStateRequest body = null,
            CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["nextState"] = nextState,
                ["getTicket"] = getTicket,
                ["sendTicketStateChangedMessage"] = sendTicketStateChangedMessage,
                ["enableWebhook"] = enableWebhook
            };

            var queryString = BuildQueryString(queryParams);
            var url = $"{_baseUrl}/tickets/setstate/{ticketId}?{queryString}";

            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Put,
                url,
                body,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Updates the fields of a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="request">The fields to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetTicketFieldsAsync(
            int ticketId,
            SetTicketFieldsRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/setfields/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Adds tags to a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="tags">Tags to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated list of tags</returns>
        public async Task<(bool Success, List<string> Data, string Error)> AddTicketTagsAsync(
            int ticketId,
            List<string> tags,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<string>>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/addtags/{ticketId}",
                tags,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Removes a tag from a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="tag">Tag to remove</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated list of tags</returns>
        public async Task<(bool Success, List<string> Data, string Error)> RemoveTicketTagAsync(
            int ticketId,
            string tag,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<string>>(
                HttpMethod.Delete,
                $"{_baseUrl}/tickets/removetag/{ticketId}?tag={Uri.EscapeDataString(tag)}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        // Users

        /// <summary>
        /// Gets all users in the department
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of users</returns>
        public async Task<(bool Success, List<User> Data, string Error)> GetAllUsersAsync(
            CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, string Error)> SetUserStatusAsync(
            User.UserStatus nextStatus,
            CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<UserStatusResponse>(
                HttpMethod.Get,
                $"{_baseUrl}/users/getstatus",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        // Contacts

        /// <summary>
        /// Gets a contact by ID
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested contact</returns>
        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(
            Guid contactId,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Contact>(
                HttpMethod.Get,
                $"{_baseUrl}/contacts/get/{contactId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets the name of a contact
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="nextName">The new name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, MessageResponse Data, string Error)> SetContactNameAsync(
            Guid contactId,
            string nextName,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/contacts/setname/{contactId}",
                new { nextName },
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        // Protocol

        /// <summary>
        /// Sends a message through a protocol (WhatsApp, SMS, etc.)
        /// </summary>
        /// <param name="request">The message to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sent message</returns>
        public async Task<(bool Success, Message Data, string Error)> SendProtocolMessageAsync(
            Message request,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Message>(
                HttpMethod.Post,
                $"{_baseUrl}/protocols/send",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        // Webhooks

        /// <summary>
        /// Gets webhook events
        /// </summary>
        /// <param name="deleteEvents">Whether to delete events after retrieving them</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of webhook events</returns>
        public async Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(
            bool deleteEvents = true,
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<WebhookEvent>>(
                HttpMethod.Get,
                $"{_baseUrl}/webhooks/getevents?deleteEvents={deleteEvents.ToString().ToLower()}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        #endregion
    }
}