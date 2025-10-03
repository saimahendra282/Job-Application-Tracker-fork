namespace JobApplicationTrackerApi.Utils;

/// <summary>
/// Represents a standardized API response structure
/// </summary>
/// <typeparam name="T">The type of data included in the response</typeparam>
public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; }

    /// <summary>
    /// Creates a successful API response
    /// </summary>
    /// <param name="data">The data to include in the response</param>
    /// <param name="message">Optional success message</param>
    /// <returns>A successful ApiResponse instance</returns>
    public static ApiResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a failed API response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>A failed ApiResponse instance</returns>
    public static ApiResponse<T> Failure(string message)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = default!,
            Timestamp = DateTime.UtcNow
        };
    }
}