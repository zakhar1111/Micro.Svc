using System.Text.Json;

namespace Shopping.Aggregator.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            /* TODO - Stream reading
            using var contentStream = await response.Content.ReadAsStreamAsync();
            var dataAsString = await JsonSerializer.DeserializeAsync<T>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return dataAsString;
            */
        }
    }
}
