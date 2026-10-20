using System.Net.Http.Json;
using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;


namespace BBLiftClient.Application.Services;

public class AuthService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    public async Task<Result> RegisterAsync(SignUpRequest request)
    {
        var response = await client.PostAsJsonAsync("auth/signup", request);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>();

        return Result.Success();

    }

    public async Task<Result<TokenModel>> LoginAsync(SignInRequest request)
    {
        var response = await client.PostAsJsonAsync("auth/signin", request);

        return await response.Content.ReadFromJsonAsync<Result<TokenModel>>();
    }

    public async Task<Result<TokenModel>> RefreshAsync(TokenModel model)
    {
        var response = await client.PostAsJsonAsync("auth/refresh", model);

        return await response.Content.ReadFromJsonAsync<Result<TokenModel>>();
    }

    public async Task<Result> RevokeAsync(string userId)
    {
        var path = Path.Combine("auth/revoke", userId);

        var response = await client.GetAsync(path);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>();

        return Result.Success();
    }
    public async Task<Result<UserProfileResponse>> GetProfileAsync()
    {
        var response = await client.GetAsync("auth/profile");
        if (!response.IsSuccessStatusCode)
            return Result<UserProfileResponse>.Failure("Profil bilgileri alınamadı.");

        return (await response.Content.ReadFromJsonAsync<Result<UserProfileResponse>>())!;
    }

    public async Task<Result> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var response = await client.PutAsJsonAsync("auth/profile", request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return Result.Failure(string.IsNullOrWhiteSpace(errorContent) ? "Profil güncellenemedi." : errorContent);
        }
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var response = await client.PutAsJsonAsync("auth/change-password", request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return Result.Failure(string.IsNullOrWhiteSpace(errorContent) ? "Şifre değiştirilemedi." : errorContent);
        }
        return Result.Success();
    }
}