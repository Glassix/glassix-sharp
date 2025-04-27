using GlassixSharp.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data;

namespace GlassixSharp
{
    public abstract class BaseGlassixClient
    {
        protected static readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly Credentials _credentials;
        private static bool wasHttpInitialized = false;

        protected readonly Dictionary<string, string> _customHeaders = new Dictionary<string, string>();
        protected static readonly ConcurrentDictionary<string, (string Token, DateTime ExpiresAt)> _tokens = new ConcurrentDictionary<string, (string, DateTime)>();
        protected static readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);

        static BaseGlassixClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Creates a new instance of the GlassixSharp client
        /// </summary>
        /// <param name="credentials">The credentials to use for authentication</param>
        /// <param name="headers">Custom headers that will be sent on every request</param>
        public BaseGlassixClient(Credentials credentials, Dictionary<string, string>? headers = null)
        {
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));

            string glassixDomain = "glassix.com";
            if (credentials.IsTestingEnvironment)
            {
                glassixDomain = "glassix-dev.com";
            }
            _baseUrl = $"https://{_credentials.WorkspaceName}.{glassixDomain}/api/v1.2";

            
            if(!wasHttpInitialized)
            {
                if (_credentials.TimeoutSeconds > 0)
                {
                    _httpClient.Timeout = TimeSpan.FromSeconds(_credentials.TimeoutSeconds);
                }
            }

            wasHttpInitialized = true;

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
        protected async Task<ApiResponse<T>> SendRequestAsync<T>(HttpMethod method, string url, object body = null, bool requiresAuth = true, CancellationToken cancellationToken = default)
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
        protected static string BuildQueryString(Dictionary<string, object> parameters)
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
    }
}
