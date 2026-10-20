using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;
using BBLift.Application.Dtos;

namespace BBLiftClient.Application.Services;

public class WorkoutSessionService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/workout-sessions";

    public async Task<WorkoutSessionResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<WorkoutSessionResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<WorkoutSessionResponse>> GetAllByUserIdAsync(string userId)
    {
        var path = $"{ApiPath}/get-all-by-user-id?id={userId}";
        return await GetAsync<IEnumerable<WorkoutSessionResponse>>(path) ?? [];
    }

    public async Task<Guid?> CreateAndReturnIdAsync(CreateWorkoutSessionRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (response.IsSuccessStatusCode)
        {
            var createdSession = await response.Content.ReadFromJsonAsync<WorkoutSessionResponse>();
            return createdSession?.Id;
        }

        return null;
    }

    public async Task<Result> CompleteSessionAsync(Guid sessionId)
    {
        var updateRequest = new UpdateWorkoutSessionRequest(
            Id: sessionId, 
            Name: null, 
            EndTime: DateTimeOffset.UtcNow, 
            WorkOutDurationInSeconds: 0, 
            Status: BBLiftClient.Application.Enums.WorkoutStatus.Completed
        );

        var response = await client.PutAsJsonAsync($"{ApiPath}/{sessionId}", updateRequest);

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("Seans sonlandırılamadı.");
            }
            catch
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return Result.Failure(string.IsNullOrWhiteSpace(errorText) ? $"API Hatası ({response.StatusCode})" : errorText);
            }
        }

        return Result.Success();
    }

    public async Task<Result> CreateAsync(CreateWorkoutSessionRequest request)
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

    public async Task<Result> UpdateAsync(UpdateWorkoutSessionRequest request)
    {
        var response = await client.PutAsJsonAsync($"{ApiPath}/{request.Id}", request);

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

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await client.DeleteAsync($"{ApiPath}/{id}");

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result>() ?? Result.Failure("Bir hata oluştu.");

        return Result.Success();
    }
}