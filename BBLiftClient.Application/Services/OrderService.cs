using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class OrderService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/orders";

    public async Task<OrderResponse?> GetByIdAsync(Guid id)
    {
        return await GetAsync<OrderResponse?>($"{ApiPath}/{id}");
    }

    public async Task<IEnumerable<OrderResponse>> GetAllByUserIdAsync(string userId)
    {
        return await GetAsync<IEnumerable<OrderResponse>>(
            $"{ApiPath}/get-all-by-user-id?userId={Uri.EscapeDataString(userId)}") ?? [];
    }

    public async Task<Result<OrderResponse>> CreateAsync(CreateOrderRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (!response.IsSuccessStatusCode)
            return Result<OrderResponse>.Failure(await ReadErrorAsync(response));

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();

        return order is null
            ? Result<OrderResponse>.Failure("Sipariş oluşturuldu ancak veri okunamadı.")
            : Result<OrderResponse>.Success(order);
    }

    public async Task<Result> UpdateStatusAsync(Guid id, UpdateOrderRequest request)
    {
        var response = await client.PutAsJsonAsync($"{ApiPath}/{id}", request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }
}