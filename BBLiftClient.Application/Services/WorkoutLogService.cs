using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;
using BBLift.Application.Dtos;

namespace BBLiftClient.Application.Services;

public class WorkoutLogService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/workout-logs";

    public async Task<WorkoutLogResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<WorkoutLogResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<WorkoutLogResponse>> GetBySessionIdAsync(Guid sessionId)
    {
        var response = await GetAsync<IEnumerable<WorkoutLogResponse>>($"{ApiPath}/by-session/{sessionId}");
        return response ?? [];
    }

    public async Task<IEnumerable<WorkoutLogResponse>> GetAllByUserIdAsync(string userId)
    {
        return await GetAsync<IEnumerable<WorkoutLogResponse>>($"{ApiPath}/get-by-user-id/{userId}") ?? [];
    }

    public async Task<Result> CreateAsync(CreateWorkoutLogRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("API'den hata döndü.");
            }
            catch
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return Result.Failure(string.IsNullOrWhiteSpace(errorText) ? "API Hatası" : errorText);
            }
        }

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateWorkoutLogRequest request)
    {
        var response = await client.PutAsJsonAsync($"{ApiPath}/{id}", request);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("Bir hata oluştu.");

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await client.DeleteAsync($"{ApiPath}/{id}");

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("Bir hata oluştu.");

        return Result.Success();
    }
}