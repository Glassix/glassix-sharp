using System.Text.Json.Serialization;

namespace GlassixSharp.Models.Responses
{
    /// <summary>
    /// Response object from the token endpoint
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// The access token to use for API requests
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Type of token (usually "bearer")
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        
        /// <summary>
        /// Token expiration time in seconds
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
