using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
    public class GlassixSharp
    {
        private static readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly GlassixCredentials _credentials;
        
        private static readonly ConcurrentDictionary<string, (string Token, DateTime ExpiresAt)> _tokens = new ConcurrentDictionary<string, (string, DateTime)>();
        private static readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);

        static GlassixSharp()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Creates a new instance of the GlassixSharp client
        /// </summary>
        /// <param name="credentials">The credentials to use for authentication</param>
        public GlassixSharp(GlassixCredentials credentials)
        {
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            _baseUrl = $"https://{_credentials.WorkspaceName}.glassix.com/api/v1.2";
            
            if (_credentials.TimeoutSeconds > 0)
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(_credentials.TimeoutSeconds);
            }
        }

        /// <summary>
        /// Gets an access token for the Glassix API
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The access token</returns>
        private async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            var tokenKey = $"{_credentials.WorkspaceName}:{_credentials.ApiKey}:{_credentials.UserName}";
            
            if (_tokens.TryGetValue(tokenKey, out var tokenInfo) && DateTime.UtcNow < tokenInfo.ExpiresAt.AddMinutes(-5))
                return tokenInfo.Token;

            await _tokenSemaphore.WaitAsync(cancellationToken);
            
            try
            {
                // Double-check after acquiring the semaphore
                if (_tokens.TryGetValue(tokenKey, out tokenInfo) && DateTime.UtcNow < tokenInfo.ExpiresAt.AddMinutes(-5))
                    return tokenInfo.Token;

                var request = new TokenRequest
                {
                    ApiKey = _credentials.ApiKey.ToString(),
                    ApiSecret = _credentials.ApiSecret,
                    UserName = _credentials.UserName
                };

                var response = await SendRequestAsync<TokenResponse>(
                    HttpMethod.Post, 
                    $"{_baseUrl}/token/get", 
                    request, 
                    false, 
                    cancellationToken);
                
                if (response.Success)
                {
                    var expiresAt = DateTime.UtcNow.AddSeconds(response.Data.ExpiresIn);
                    _tokens[tokenKey] = (response.Data.AccessToken, expiresAt);
                    return response.Data.AccessToken;
                }
                
                throw new Exception($"Failed to obtain token: {response.Error}");
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }

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
                var request = new HttpRequestMessage(method, url);
                
                if (requiresAuth)
                {
                    var token = await GetTokenAsync(cancellationToken);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                if (body != null)
                {
                    var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(EmptyResponse))
                    {
                        return ApiResponse<T>.Success((T)(object)new EmptyResponse());
                    }
                    
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<T>(content, options);
                    return ApiResponse<T>.Success(result);
                }
                
                string errorMessage;
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                    errorMessage = errorResponse?.Message ?? response.ReasonPhrase;
                }
                catch
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
        private string BuildQueryString(Dictionary<string, object> parameters)
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
        public async Task<(bool Success, TicketResponse Data, string Error)> CreateTicketAsync(
            CreateTicketRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<TicketResponse>(
                HttpMethod.Post, 
                $"{_baseUrl}/tickets/create", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        /// <summary>
        /// Gets a ticket by ID
        /// </summary>
        /// <param name="ticketId">ID of the ticket to get</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested ticket</returns>
        public async Task<(bool Success, TicketResponse Data, string Error)> GetTicketAsync(
            int ticketId, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<TicketResponse>(
                HttpMethod.Get, 
                $"{_baseUrl}/tickets/get/{ticketId}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
            TicketState? ticketState = null, 
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        /// <summary>
        /// Sends a message in a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="request">The message to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The message transaction</returns>
        public async Task<(bool Success, TransactionResponse Data, string Error)> SendMessageAsync(
            int ticketId, 
            SendMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<TransactionResponse>(
                HttpMethod.Post, 
                $"{_baseUrl}/tickets/send/{ticketId}", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
            TicketState nextState, 
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        /// <summary>
        /// Updates the fields of a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="request">The fields to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, EmptyResponse Data, string Error)> SetTicketFieldsAsync(
            int ticketId, 
            SetTicketFieldsRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put, 
                $"{_baseUrl}/tickets/setfields/{ticketId}", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        /// <summary>
        /// Sets the status of the current user
        /// </summary>
        /// <param name="nextStatus">The new status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, EmptyResponse Data, string Error)> SetUserStatusAsync(
            UserStatus nextStatus, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put, 
                $"{_baseUrl}/users/setstatus", 
                new { nextStatus = nextStatus.ToString() }, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Protocol

        /// <summary>
        /// Sends a message through a protocol (WhatsApp, SMS, etc.)
        /// </summary>
        /// <param name="request">The message to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sent message</returns>
        public async Task<(bool Success, SendProtocolMessageResponse Data, string Error)> SendProtocolMessageAsync(
            SendProtocolMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<SendProtocolMessageResponse>(
                HttpMethod.Post, 
                $"{_baseUrl}/protocols/send", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        #endregion
    }
}etId, 
            string nextState, 
            bool getTicket = false, 
            bool sendTicketStateChangedMessage = true,
            bool enableWebhook = true,
            SetTicketStateRequest body = null,
            CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["nextState"] = nextState,
                ["getTicket"] = getTicket.ToString().ToLower(),
                ["sendTicketStateChangedMessage"] = sendTicketStateChangedMessage.ToString().ToLower(),
                ["enableWebhook"] = enableWebhook.ToString().ToLower()
            };
            
            var queryString = BuildQueryString(BuildQueryParams(queryParams));
            var url = $"{_baseUrl}/tickets/setstate/{ticketId}?{queryString}";
            
            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Put, 
                url, 
                body, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, EmptyResponse Data, string Error)> SetTicketFieldsAsync(
            int ticketId, 
            SetTicketFieldsRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put, 
                $"{_baseUrl}/tickets/setfields/{ticketId}", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Users

        public async Task<(bool Success, List<User> Data, string Error)> GetAllUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<User>>(
                HttpMethod.Get, 
                $"{_baseUrl}/users/allusers", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, EmptyResponse Data, string Error)> SetUserStatusAsync(
            string nextStatus, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put, 
                $"{_baseUrl}/users/setstatus", 
                new { nextStatus }, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<UserStatusResponse>(
                HttpMethod.Get, 
                $"{_baseUrl}/users/getstatus", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Contacts

        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(
            Guid contactId, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Contact>(
                HttpMethod.Get, 
                $"{_baseUrl}/contacts/get/{contactId}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Protocol

        public async Task<(bool Success, SendProtocolMessageResponse Data, string Error)> SendProtocolMessageAsync(
            SendProtocolMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<SendProtocolMessageResponse>(
                HttpMethod.Post, 
                $"{_baseUrl}/protocols/send", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Webhooks

        public async Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(
            bool deleteEvents = true, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<WebhookEvent>>(
                HttpMethod.Get, 
                $"{_baseUrl}/webhooks/getevents?deleteEvents={deleteEvents.ToString().ToLower()}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        #endregion
    }
}
        }

        // Users

        public async Task<(bool Success, List<User> Data, string Error)> GetAllUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<User>>(
                HttpMethod.Get, 
                $"{_baseUrl}/users/allusers", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, EmptyResponse Data, string Error)> SetUserStatusAsync(
            string nextStatus, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put, 
                $"{_baseUrl}/users/setstatus", 
                new { nextStatus }, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<UserStatusResponse>(
                HttpMethod.Get, 
                $"{_baseUrl}/users/getstatus", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Contacts

        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(
            Guid contactId, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Contact>(
                HttpMethod.Get, 
                $"{_baseUrl}/contacts/get/{contactId}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Protocol

        public async Task<(bool Success, SendProtocolMessageResponse Data, string Error)> SendProtocolMessageAsync(
            SendProtocolMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<SendProtocolMessageResponse>(
                HttpMethod.Post, 
                $"{_baseUrl}/protocols/send", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Webhooks

        public async Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(
            bool deleteEvents = true, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<WebhookEvent>>(
                HttpMethod.Get, 
                $"{_baseUrl}/webhooks/getevents?deleteEvents={deleteEvents.ToString().ToLower()}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        #endregion
    }
}
        }

        // Users

        public async Task<(bool Success, List<User> Data, string Error)> GetAllUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<User>>(
                HttpMethod.Get, 
                $"{_baseUrl}/users/allusers", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, EmptyResponse Data, string Error)> SetUserStatusAsync(
            string nextStatus, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put, 
                $"{_baseUrl}/users/setstatus", 
                new { nextStatus }, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<UserStatusResponse>(
                HttpMethod.Get, 
                $"{_baseUrl}/users/getstatus", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Contacts

        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(
            Guid contactId, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Contact>(
                HttpMethod.Get, 
                $"{_baseUrl}/contacts/get/{contactId}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Protocol

        public async Task<(bool Success, SendProtocolMessageResponse Data, string Error)> SendProtocolMessageAsync(
            SendProtocolMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<SendProtocolMessageResponse>(
                HttpMethod.Post, 
                $"{_baseUrl}/protocols/send", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Webhooks

        public async Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(
            bool deleteEvents = true, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<WebhookEvent>>(
                HttpMethod.Get, 
                $"{_baseUrl}/webhooks/getevents?deleteEvents={deleteEvents.ToString().ToLower()}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        #endregion
    }
}

        public async Task<(bool Success, EmptyResponse Data, string Error)> SetUserStatusAsync(
            string nextStatus, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put, 
                $"{_baseUrl}/users/setstatus", 
                new { nextStatus }, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        public async Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<UserStatusResponse>(
                HttpMethod.Get, 
                $"{_baseUrl}/users/getstatus", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Contacts

        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(
            Guid contactId, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Contact>(
                HttpMethod.Get, 
                $"{_baseUrl}/contacts/get/{contactId}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

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
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Protocol

        public async Task<(bool Success, SendProtocolMessageResponse Data, string Error)> SendProtocolMessageAsync(
            SendProtocolMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<SendProtocolMessageResponse>(
                HttpMethod.Post, 
                $"{_baseUrl}/protocols/send", 
                request, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Webhooks

        public async Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(
            bool deleteEvents = true, 
            CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<WebhookEvent>>(
                HttpMethod.Get, 
                $"{_baseUrl}/webhooks/getevents?deleteEvents={deleteEvents.ToString().ToLower()}", 
                null, 
                true, 
                cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        #endregion
    }
}
com/api/v1.2/users/allusers";
            var response = await GetAsync<List<User>>(url, cancellationToken: cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        /// <summary>
        /// Sets the status of the current user
        /// </summary>
        public async Task<(bool Success, EmptyResponse Data, string Error)> SetUserStatusAsync(
            string nextStatus, 
            CancellationToken cancellationToken = default)
        {
            var url = $"https://{_workspaceName}.glassix.com/api/v1.2/users/setstatus";
            var response = await PutAsync<EmptyResponse>(url, new { nextStatus }, cancellationToken: cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        /// <summary>
        /// Gets the status of the current user
        /// </summary>
        public async Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(
            CancellationToken cancellationToken = default)
        {
            var url = $"https://{_workspaceName}.glassix.com/api/v1.2/users/getstatus";
            var response = await GetAsync<UserStatusResponse>(url, cancellationToken: cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Contacts API

        /// <summary>
        /// Gets a contact by ID
        /// </summary>
        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(
            Guid contactId, 
            CancellationToken cancellationToken = default)
        {
            var url = $"https://{_workspaceName}.glassix.com/api/v1.2/contacts/get/{contactId}";
            var response = await GetAsync<Contact>(url, cancellationToken: cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        /// <summary>
        /// Sets the name of a contact
        /// </summary>
        public async Task<(bool Success, MessageResponse Data, string Error)> SetContactNameAsync(
            Guid contactId, 
            string nextName, 
            CancellationToken cancellationToken = default)
        {
            var url = $"https://{_workspaceName}.glassix.com/api/v1.2/contacts/setname/{contactId}";
            var response = await PutAsync<MessageResponse>(url, new { nextName }, cancellationToken: cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Protocol API

        /// <summary>
        /// Sends a message through a protocol (WhatsApp, SMS, etc.)
        /// </summary>
        public async Task<(bool Success, SendProtocolMessageResponse Data, string Error)> SendProtocolMessageAsync(
            SendProtocolMessageRequest request, 
            CancellationToken cancellationToken = default)
        {
            var url = $"https://{_workspaceName}.glassix.com/api/v1.2/protocols/send";
            var response = await PostAsync<SendProtocolMessageResponse>(url, request, cancellationToken: cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        // Webhooks API

        /// <summary>
        /// Gets webhook events
        /// </summary>
        public async Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(
            bool deleteEvents = true, 
            CancellationToken cancellationToken = default)
        {
            var url = $"https://{_workspaceName}.glassix.com/api/v1.2/webhooks/getevents?deleteEvents={deleteEvents.ToString().ToLower()}";
            var response = await GetAsync<List<WebhookEvent>>(url, cancellationToken: cancellationToken);
            
            return (response.Success, response.Data, response.Error);
        }

        #endregion
    }
}
