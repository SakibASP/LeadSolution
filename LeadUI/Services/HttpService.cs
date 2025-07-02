using LeadUI.Interfaces;
using System.Net.Http.Headers;

namespace LeadUI.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
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


        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
