using System;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Credentials for authenticating with the Glassix API
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// The Glassix workspace name
        /// </summary>
        public string WorkspaceName { get; }
        
        /// <summary>
        /// The username (email) for authentication
        /// </summary>
        public string UserName { get; }
        
        /// <summary>
        /// The API key for authentication
        /// </summary>
        public Guid ApiKey { get; }
        
        /// <summary>
        /// The API secret for authentication
        /// </summary>
        public string ApiSecret { get; }
        
        /// <summary>
        /// Timeout for HTTP requests in seconds (default: 60 seconds)
        /// </summary>
        public int TimeoutSeconds { get; }
        public bool IsTestingEnvironment { get; }

        /// <summary>
        /// Creates a new instance of GlassixCredentials
        /// </summary>
        /// <param name="workspaceName">The Glassix workspace name</param>
        /// <param name="userName">The username (email) for authentication</param>
        /// <param name="apiKey">The API key for authentication</param>
        /// <param name="apiSecret">The API secret for authentication</param>
        /// <param name="timeoutSeconds">Timeout for HTTP requests in seconds (default: 60 seconds)</param>
        public Credentials(
            string workspaceName, 
            string userName, 
            Guid apiKey, 
            string apiSecret,
            int timeoutSeconds = 60, bool isTestingEnvironment = false)
        {
            if (string.IsNullOrEmpty(workspaceName))
                throw new ArgumentNullException(nameof(workspaceName));
            
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            
            if (apiKey == Guid.Empty)
                throw new ArgumentException("API key cannot be empty", nameof(apiKey));
            
            if (string.IsNullOrEmpty(apiSecret))
                throw new ArgumentNullException(nameof(apiSecret));
            
            if (timeoutSeconds <= 0)
                throw new ArgumentException("Timeout must be greater than 0", nameof(timeoutSeconds));

            WorkspaceName = workspaceName;
            UserName = userName;
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            TimeoutSeconds = timeoutSeconds;
            IsTestingEnvironment = isTestingEnvironment;
        }
    }
}
