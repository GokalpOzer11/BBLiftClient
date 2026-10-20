using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class RoutineService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/routines";

    public async Task<RoutineResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<RoutineResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<RoutineResponse>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<RoutineResponse>>(ApiPath) ?? [];
    }

    public async Task<IEnumerable<RoutineResponse>> GetAllByUserIdAsync(string userId)
    {
        var path = $"{ApiPath}/get-all-by-user-id?id={Uri.EscapeDataString(userId)}";
        return await GetAsync<IEnumerable<RoutineResponse>>(path) ?? [];
    }

    public async Task<Result<RoutineResponse>> CreateAsync(CreateRoutineRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (!response.IsSuccessStatusCode)
            return Result<RoutineResponse>.Failure(await ReadErrorAsync(response));

        var routine = await response.Content.ReadFromJsonAsync<RoutineResponse>();

        return routine is null
            ? Result<RoutineResponse>.Failure("Kayıt başarılı ancak veri okunamadı.")
            : Result<RoutineResponse>.Success(routine);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateRoutineRequest request)
    {
        var response = await client.PutAsJsonAsync($"{ApiPath}/{id}", request);

        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadErrorAsync(response));

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await client.DeleteAsync($"{ApiPath}/{id}");

        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadErrorAsync(response));

        return Result.Success();
    }
}