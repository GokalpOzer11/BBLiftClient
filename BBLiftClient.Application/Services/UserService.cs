using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;

namespace BBLiftClient.Application.Services;

public class UserService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/users";

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        return await GetAsync<IEnumerable<UserResponse>>(ApiPath) ?? [];
    }

    public async Task<Result> AddToRoleAsync(Guid userId, string role)
    {
        var response = await client.PostAsync(
            $"{ApiPath}/{userId}/roles/{Uri.EscapeDataString(role)}", null);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> RemoveFromRoleAsync(Guid userId, string role)
    {
        var response = await client.DeleteAsync(
            $"{ApiPath}/{userId}/roles/{Uri.EscapeDataString(role)}");

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }
}