using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class MembershipPackageService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/membership-packages";

    public async Task<MembershipPackageResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<MembershipPackageResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<MembershipPackageResponse>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<MembershipPackageResponse>>(ApiPath) ?? [];
    }

    public async Task<Result> CreateAsync(CreateMembershipPackageRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateMembershipPackageRequest request)
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