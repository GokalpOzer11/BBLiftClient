namespace BBLiftClient.Application.Dtos;

public record CartResponse
(
    Guid Id,
    Guid AppUserId,
    IEnumerable<CartItemResponse> Items
);