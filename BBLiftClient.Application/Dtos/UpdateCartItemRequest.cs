namespace BBLiftClient.Application.Dtos;

public record UpdateCartItemRequest
(
    Guid Id,
    int Quantity
);