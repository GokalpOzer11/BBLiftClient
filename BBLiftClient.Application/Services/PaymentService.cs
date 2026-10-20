using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using System.Net.Http.Json;

namespace BBLiftClient.Application.Services;

public class PaymentService(IHttpClientFactory factory)
    : ServiceBase(factory)
{
    private const string ApiPath = "api/payments";

    public async Task<Result<Guid>> CreateAsync(CreatePaymentRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiPath, request);

        if (!response.IsSuccessStatusCode)
            return Result<Guid>.Failure(await ReadErrorAsync(response));

        var created = await response.Content.ReadFromJsonAsync<CreatedIdResponse>();

        return created is null
            ? Result<Guid>.Failure("Ödeme oluşturuldu ancak veri okunamadı.")
            : Result<Guid>.Success(created.Id);
    }

    public async Task<Result> UpdateStatusAsync(Guid id, UpdatePaymentRequest request)
    {
        var response = await client.PutAsJsonAsync($"{ApiPath}/{id}", request);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(await ReadErrorAsync(response));
    }
}