namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a generic API response
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the API request was successful
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// The data returned by the API (null if the request failed)
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Error message (null if the request succeeded)
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// The HTTP status code of the response
        /// </summary>
        public int? StatusCode { get; private set; }

        private ApiResponse(bool isSuccess, T data, string errorMessage, int? statusCode)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public static ApiResponse<T> Success(T data) => new ApiResponse<T>(true, data, null, 200);

        public static ApiResponse<T> Error(string errorMessage, int? statusCode = null) => new ApiResponse<T>(false, default, errorMessage, statusCode);
    }
}