using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class FavoriteExerciseService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/favorite-exercises";

    public async Task<IEnumerable<FavoriteExerciseResponse>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<FavoriteExerciseResponse>>(ApiPath) ?? [];
    }

    public async Task<FavoriteExerciseResponse?> GetByIdAsync(Guid id)
    {
        var path = Path.Combine(ApiPath, id.ToString());

        return await GetAsync<FavoriteExerciseResponse?>(path);
    }

    public async Task<Result> CreateAsync(CreateFavoriteExerciseRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("An error occurred.");

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var path = Path.Combine(ApiPath, id.ToString());

        var response = await client.DeleteAsync(path);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("An error occurred.");

        return Result.Success();
    }
}