using System;

namespace GlassixSharp.Utilities
{
    /// <summary>
    /// Exception thrown when an error occurs during API communication
    /// </summary>
    public class GlassixApiException : Exception
    {
        /// <summary>
        /// HTTP status code if available
        /// </summary>
        public int? StatusCode { get; }

        public GlassixApiException(string message, int? statusCode = null) 
            : base(message)
        {
            StatusCode = statusCode;
        }

        public GlassixApiException(string message, Exception innerException, int? statusCode = null) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
