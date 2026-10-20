using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class EquipmentService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/equipments";

    public async Task<EquipmentResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<EquipmentResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<EquipmentResponse>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<EquipmentResponse>>(ApiPath) ?? [];
    }

    public async Task<Result> CreateAsync(CreateEquipmentRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateEquipmentRequest request)
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