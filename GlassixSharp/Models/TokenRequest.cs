using System.Text.Json.Serialization;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Request object for obtaining an access token
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// Your API key
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }
        
        /// <summary>
        /// Your API secret
        /// </summary>
        [JsonPropertyName("apiSecret")]
        public string ApiSecret { get; set; }
        
        /// <summary>
        /// User's email address
        /// </summary>
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
    }
}
