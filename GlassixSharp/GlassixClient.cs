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
                    if (string.IsNullOrEmpty(response.Data.access_token))
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
        private async Task<ApiResponse<T>> SendRequestAsync<T>(HttpMethod method, string url, object body = null, bool requiresAuth = true, CancellationToken cancellationToken = default)
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
                if (_customHeaders != null && _customHeaders.Count > 0)
                {
                    foreach (var header in _customHeaders)
                    {
                        if (!string.IsNullOrEmpty(header.Key) && !string.IsNullOrEmpty(header.Value))
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

                    // Check the Content-Type header to determine how to process the response
                    string contentType = response.Content.Headers.ContentType?.MediaType?.ToLowerInvariant();

                    if (contentType == "text/html" && typeof(T) == typeof(string))
                    {
                        var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return ApiResponse<T>.Success((T)(object)html);
                    }

                    if (contentType == "application/pdf" && typeof(T) == typeof(byte[]))
                    {
                        var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        return ApiResponse<T>.Success((T)(object)bytes);
                    }


                    var result = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
                    return ApiResponse<T>.Success(result);
                }

                string errorMessage = response.ReasonPhrase;
                if (!string.IsNullOrEmpty(content))
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

        #region Tickets

        /// <summary>
        /// Creates a new ticket
        /// </summary>
        /// <param name="request">The ticket creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created ticket</returns>
        public async Task<(bool Success, Ticket Data, string Error)> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, Ticket Data, string Error)> GetTicketAsync(int ticketId, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, TicketListResponse Data, string Error)> ListTicketsAsync(DateTime since, DateTime until, Ticket.State? ticketState = null, SortOrder? sortOrder = null, string page = null, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, Transaction Data, string Error)> SendMessageAsync(int ticketId, SendMessageRequest request, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, MessageResponse Data, string Error)> SetTicketStateAsync(int ticketId, Ticket.State nextState, bool getTicket = false, bool sendTicketStateChangedMessage = true, bool enableWebhook = true, SetTicketStateRequest body = null, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, string Error)> SetTicketFieldsAsync(int ticketId, SetTicketFieldsRequest request, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, List<string> Data, string Error)> AddTicketTagsAsync(int ticketId, List<string> tags, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, List<string> Data, string Error)> RemoveTicketTagAsync(int ticketId, string tag, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<string>>(
                HttpMethod.Delete,
                $"{_baseUrl}/tickets/removetag/{ticketId}?tag={Uri.EscapeDataString(tag)}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Updates a participant's name within a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="participantId">ID of the participant in the ticket</param>
        /// <param name="name">New name for the participant</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetParticipantNameAsync(int ticketId, int participantId, string name, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                id = participantId,
                name = name
            };

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/setparticipantname/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Assigns a new owner to a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="keepCurrentOwnerInConversation">Whether to keep the current owner as a participant</param>
        /// <param name="nextOwnerUserName">Email address of the new owner</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetTicketOwnerAsync(int ticketId, string nextOwnerUserName, bool keepCurrentOwnerInConversation = false, CancellationToken cancellationToken = default)
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>
            {
                ["keepCurrentOwnerInConversation"] = keepCurrentOwnerInConversation,
                ["nextOwnerUserName"] = nextOwnerUserName
            };

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/tickets/setowner/{ticketId}?{queryString}";

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Assigns an available user to a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> AssignAvailableUserAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/assignavailableuser/{ticketId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Moves a ticket to another department
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="departmentId">ID of the target department</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>ID of the new ticket in the target department</returns>
        public async Task<(bool Success, int NewTicketId, string Error)> SetDepartmentAsync(int ticketId, Guid departmentId, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                departmentId = departmentId.ToString()
            };

            var response = await SendRequestAsync<SetDepartmentResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/setdepartment/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data?.ticketId ?? 0, response.ErrorMessage);
        }

        /// <summary>
        /// Adds a note to a ticket (visible only to agents)
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="text">Text content of the note</param>
        /// <param name="html">HTML content of the note</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> AddNoteAsync(int ticketId, string text = null, string html = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(html))
            {
                throw new ArgumentException("Either text or html must be provided.");
            }

            var request = new
            {
                text = text,
                html = html
            };

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/addnote/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Permanently deletes (scrambles) a ticket's data
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> ScrambleTicketAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Delete,
                $"{_baseUrl}/tickets/scramble/{ticketId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Generates a PDF document of the ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="includeDetails">Whether to include ticket details</param>
        /// <param name="includeConversationLink">Whether to include conversation link</param>
        /// <param name="includeNotes">Whether to include notes</param>
        /// <param name="replaceContentId">Whether to replace content IDs</param>
        /// <param name="showParticipantType">Whether to show participant types</param>
        /// <param name="fontSizeInPixels">Font size in pixels</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>PDF file as byte array</returns>
        public async Task<(bool Success, byte[] PdfData, string Error)> GetTicketPdfAsync(int ticketId, TicketRenderOptions ticketRenderOptions, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<byte[]>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/pdf/{ticketId}",
                ticketRenderOptions,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Generates an HTML document of the ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="includeDetails">Whether to include ticket details</param>
        /// <param name="includeConversationLink">Whether to include conversation link</param>
        /// <param name="includeNotes">Whether to include notes</param>
        /// <param name="fontSizeInPixels">Font size in pixels</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>HTML content as string</returns>
        public async Task<(bool Success, string HtmlContent, string Error)> GetTicketHtmlAsync(int ticketId, TicketRenderOptions ticketRenderOptions, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<string>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/html/{ticketId}",
                ticketRenderOptions,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Generates a survey link for a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="surveyId">ID of the survey</param>
        /// <param name="participantId">ID of the participant (0 for main participant)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The generated survey link</returns>
        public async Task<(bool Success, Uri SurveyLink, string Error)> GenerateSurveyLinkAsync(int ticketId, int surveyId, int participantId = 0, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                surveyId = surveyId,
                participantId = participantId
            };

            var response = await SendRequestAsync<SurveyLinkResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/generatesurveylink/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data?.surveyLink, response.ErrorMessage);
        }

        /// <summary>
        /// Sets a summary for a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="summary">The summary text</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetTicketSummaryAsync(int ticketId, string summary, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                summary = summary
            };

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/setsummary/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }
        #endregion

        #region Users

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
        #endregion

        #region Contacts

        /// <summary>
        /// Gets a contact by ID
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested contact</returns>
        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(Guid contactId, CancellationToken cancellationToken = default)
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
        public async Task<(bool Success, MessageResponse Data, string Error)> SetContactNameAsync(Guid contactId, string nextName, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/contacts/setname/{contactId}",
                new { nextName },
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }
        #endregion

        #region Protocol

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
        #endregion

        #region Webhooks

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

        #endregion
    }
}