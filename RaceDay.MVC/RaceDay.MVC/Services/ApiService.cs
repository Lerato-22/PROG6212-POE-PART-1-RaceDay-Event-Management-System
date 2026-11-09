using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RaceDay.MVC.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCookieHeader()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "";
            var cookies = context.Request.Cookies;
            return string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, endpoint);
            var cookieHeader = GetCookieHeader();
            if (!string.IsNullOrEmpty(cookieHeader))
                request.Headers.Add("Cookie", cookieHeader);
            if (content != null)
                request.Content = content;
            return request;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var request = CreateRequest(HttpMethod.Get, endpoint);
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return default;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = CreateRequest(HttpMethod.Post, endpoint, content);
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = CreateRequest(HttpMethod.Put, endpoint, content);
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var request = CreateRequest(HttpMethod.Delete, endpoint);
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostFormAsync(string endpoint, MultipartFormDataContent formData)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var request = CreateRequest(HttpMethod.Post, endpoint, formData);
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PutFormAsync(string endpoint, MultipartFormDataContent formData)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var request = CreateRequest(HttpMethod.Put, endpoint, formData);
            return await client.SendAsync(request);
        }
    }
}