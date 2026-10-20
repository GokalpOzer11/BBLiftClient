using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;
using System.Text.Json;

namespace BBLiftClient.Application.Services;

public class RoutineExerciseService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/routine-exercises";

    public sealed record CreatedResponse(Guid Id);

    public async Task<Result<Guid>> CreateAsync(CreateRoutineExerciseRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (!response.IsSuccessStatusCode)
            return Result<Guid>.Failure(await ReadErrorAsync(response));

        try
        {
            var created = await response.Content.ReadFromJsonAsync<CreatedResponse>();
            return Result<Guid>.Success(created?.Id ?? Guid.Empty);
        }
        catch (JsonException)
        {
            return Result<Guid>.Success(Guid.Empty);
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateRoutineExerciseRequest request)
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