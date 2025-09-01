namespace Core.Models.Common;

public class RequestLogs
{
    public int Id { get; set; }

    public string Method { get; set; } = null!;

    public string Path { get; set; } = null!;

    public string? Request { get; set; }

    public int StatusCode { get; set; }

    public string? Response { get; set; }

    public DateTime CreatedDate { get; set; }

    public string UserId { get; set; } = "Unauthorized";
}

