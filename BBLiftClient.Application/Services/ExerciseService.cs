using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class ExerciseService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/exercises";

    public async Task<ExerciseResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<ExerciseResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<ExerciseResponse>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<ExerciseResponse>>(ApiPath) ?? [];
    }

    public async Task<Result> CreateAsync(CreateExerciseRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateExerciseRequest request)
    {
        var response = await client.PutAsJsonAsync($"{ApiPath}/{id}", request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await client.DeleteAsync($"{ApiPath}/{id}");

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }
}