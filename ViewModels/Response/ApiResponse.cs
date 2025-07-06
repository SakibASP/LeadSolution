namespace Core.ViewModels.Response
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null)
            => new() { IsSuccess = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message)
            => new() { IsSuccess = false, Message = message };
    }

}
