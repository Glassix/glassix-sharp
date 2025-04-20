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
        public string apiKey { get; set; }
        
        /// <summary>
        /// Your API secret
        /// </summary>
        public string apiSecret { get; set; }
        
        /// <summary>
        /// User's email address
        /// </summary>
        public string userName { get; set; }
    }
}
