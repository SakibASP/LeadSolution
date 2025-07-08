using Lead.UI.Interfaces;
using System.Net.Http.Headers;

namespace Lead.UI.Services
{
    public class HttpService(HttpClient httpClient) : IHttpService
    {
        private readonly HttpClient _httpClient = httpClient;

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> PostAsync<T>(string version, string endpoint, object data)
        {
            var url = $"{version}/{endpoint}";
            var response = await _httpClient.PostAsJsonAsync(url, data); // Handles JSON + headers

            if (!response.IsSuccessStatusCode)
            {
                // Optionally log error:
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
}
