using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class CartService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/carts";

    public async Task<CartResponse?> GetByUserIdAsync(string userId)
    {
        return await GetAsync<CartResponse?>(
            $"{ApiPath}/get-by-user-id?userId={Uri.EscapeDataString(userId)}");
    }

    public async Task<Result> CreateAsync(CreateCartRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> AddItemAsync(CreateCartItemRequest request)
    {
        var response = await client.PostAsJsonAsync($"{ApiPath}/add-item", request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> UpdateItemAsync(UpdateCartItemRequest request)
    {
        var response = await client.PutAsJsonAsync($"{ApiPath}/update-item", request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> RemoveItemAsync(Guid id)
    {
        var response = await client.DeleteAsync($"{ApiPath}/remove-item/{id}");

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }

    public async Task<Result> ClearAsync(string userId)
    {
        var response = await client.DeleteAsync(
            $"{ApiPath}/clear-cart?userId={Uri.EscapeDataString(userId)}");

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }
}