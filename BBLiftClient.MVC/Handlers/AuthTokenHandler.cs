using System.Net.Http.Headers;

namespace BBLiftClient.MVC.Handlers;

// HttpClient ile API'ye giden her isteği araya girip yakalayacak sınıf
public class AuthTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Kullanıcının cookie'sinde sakladığımız "jwt_token" değerini buluyoruz
        var token = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "jwt_token")?.Value;

        // Eğer token varsa, API'ye giden isteğin Header'ına (Authorization: Bearer ...) ekliyoruz
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // İsteği token eklenmiş haliyle API'ye gönderiyoruz
        return await base.SendAsync(request, cancellationToken);
    }
}