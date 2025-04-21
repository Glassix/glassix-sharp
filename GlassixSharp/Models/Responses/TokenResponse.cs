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
        public string access_token { get; set; }
        
        /// <summary>
        /// Type of token (usually "bearer")
        /// </summary>
        public string token_type { get; set; }
        
        /// <summary>
        /// Token expiration time in seconds
        /// </summary>
        public double expires_in { get; set; }
    }
}
