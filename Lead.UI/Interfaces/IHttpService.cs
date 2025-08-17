namespace Lead.UI.Interfaces;

public interface IHttpService
{
    void SetBearerToken(string token);
    Task<T?> PostAsync<T>(string version, string endpoint, object data, Dictionary<string, string>? headers = null);
    Task<T?> GetAsync<T>(string version, string endpoint, Dictionary<string, string>? queryParams = null);
}
