using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class FavoriteExerciseService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/favorite-exercises";

    // Kullanıcının tüm favori egzersizlerini listelemek için
    public async Task<IEnumerable<FavoriteExerciseResponse>> GetAllAsync()
    {
        // Not: Eğer API tarafında UserId'ye göre filtrelemek için query parametresi 
        // veya path gerekiyorsa burayı GetAsync<...>(Path.Combine(ApiPath, "user", userId)) şeklinde güncelleyebilirsin.
        return await GetAsync<IEnumerable<FavoriteExerciseResponse>>(ApiPath) ?? [];
    }

    // İlgili favori kaydının detayını getirmek için
    public async Task<FavoriteExerciseResponse?> GetByIdAsync(Guid id)
    {
        var path = Path.Combine(ApiPath, id.ToString());

        return await GetAsync<FavoriteExerciseResponse?>(path);
    }

    // Egzersizi favorilere eklemek için
    public async Task<Result> CreateAsync(CreateFavoriteExerciseRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("An error occurred.");

        return Result.Success();
    }

    // Egzersizi favorilerden çıkarmak için
    public async Task<Result> DeleteAsync(Guid id)
    {
        var path = Path.Combine(ApiPath, id.ToString());

        var response = await client.DeleteAsync(path);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("An error occurred.");

        return Result.Success();
    }
}