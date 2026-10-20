using System.Net.Http.Headers;
using System.Text.Json;

namespace BBLiftClient.Application.Services;

public abstract class ServiceBase
{
    protected readonly HttpClient client;

    protected ServiceBase(IHttpClientFactory factory) => client = factory.CreateClient("BBLiftApi");

    protected void SetToken(string token) => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    protected async Task<T?> GetAsync<T>(string url)
    {
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

        if (!response.IsSuccessStatusCode) return default;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(json);
    }

    protected static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        if ((int)response.StatusCode >= 500)
            return $"Sunucuda beklenmeyen bir hata oluştu ({(int)response.StatusCode}).";

        var body = (await response.Content.ReadAsStringAsync()).Trim();

        if (string.IsNullOrEmpty(body))
            return $"İstek başarısız oldu ({(int)response.StatusCode}).";

        try
        {
            if (body.StartsWith('"'))
                return JsonSerializer.Deserialize<string>(body) ?? body;

            if (body.StartsWith('{'))
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("title", out var title)
                    && title.ValueKind == JsonValueKind.String)
                    return title.GetString() ?? body;
            }
        }
        catch (JsonException)
        {
        }

        return body.Length > 300 ? body[..300] + "…" : body;
    }
}