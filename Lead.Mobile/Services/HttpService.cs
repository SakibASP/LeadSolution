using Lead.Mobile.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Lead.Mobile.Services;

public class HttpService(HttpClient httpClient) : IHttpService
{
    private readonly HttpClient _httpClient = httpClient;

    public void SetBearerToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> PostAsync<T>(string version, string endpoint, object data, Dictionary<string, string>? headers = null)
    {
        var url = $"{version}/{endpoint}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(data) // Handles JSON serialization
        };

        // Add custom headers if provided
        if (headers != null)
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
            
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            // Optionally log error
            var err = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ API Error ({response.StatusCode}): {err}");
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }




    public async Task<T?> GetAsync<T>(string version, string endpoint, Dictionary<string, string>? queryParams = null)
    {
        var query = queryParams is not null && queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))
            : string.Empty;

        var url = $"{version}/{endpoint}{query}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ API Error ({response.StatusCode}): {err}");
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
