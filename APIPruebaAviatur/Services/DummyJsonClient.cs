using System.Net.Http;

namespace APIPruebaAviatur.Services
{
    public class DummyJsonProduct
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public double rating { get; set; }
        public String category { get; set; }
    }

    public class DummyJsonProductsResponse
    {
        public List<DummyJsonProduct> products { get; set; }
        public int total { get; set; }
        public int skip { get; set; }
        public int limit { get; set; }
    }
    public class DummyJsonClient
    {
        private readonly HttpClient _httpClient;

        public DummyJsonClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://dummyjson.com/");
        }

        public async Task<DummyJsonProductsResponse> SearchProductsAsync(string query)
        {
            try // 13
            {
                var response = await _httpClient.GetFromJsonAsync<DummyJsonProductsResponse>($"products/search?q={query}");
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al conectar con DummyJSON: {ex.Message}");
                return null;
            }
        }
    }
}
