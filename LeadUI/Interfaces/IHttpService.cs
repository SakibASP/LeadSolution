namespace LeadUI.Interfaces
{
    public interface IHttpService
    {
        void SetBearerToken(string token);
        Task<T?> PostAsync<T>(string url, object data);
        Task<T?> GetAsync<T>(string url);
    }

}
