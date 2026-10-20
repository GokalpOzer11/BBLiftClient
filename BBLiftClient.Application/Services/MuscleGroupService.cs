using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class MuscleGroupService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/muscle-groups";

    public async Task<MuscleGroupResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<MuscleGroupResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<MuscleGroupResponse>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<MuscleGroupResponse>>(ApiPath) ?? [];
    }

    public async Task<Result> CreateAsync(CreateMuscleGroupRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateMuscleGroupRequest request)
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