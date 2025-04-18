namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a generic API response
    /// </summary>
    internal class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the API request was successful
        /// </summary>
        public bool Success { get; }
        
        /// <summary>
        /// The data returned by the API (null if the request failed)
        /// </summary>
        public T Data { get; }
        
        /// <summary>
        /// Error message (null if the request succeeded)
        /// </summary>
        public string Error { get; }
        
        /// <summary>
        /// The HTTP status code of the response
        /// </summary>
        public int? StatusCode { get; }

        private ApiResponse(bool success, T data, string error, int? statusCode)
        {
            Success = success;
            Data = data;
            Error = error;
            StatusCode = statusCode;
        }

        public static ApiResponse<T> Success(T data) => new ApiResponse<T>(true, data, null, 200);
        public static ApiResponse<T> Error(string error, int? statusCode = null) => new ApiResponse<T>(false, default, error, statusCode);
    }
}
