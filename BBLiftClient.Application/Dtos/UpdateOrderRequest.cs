using BBLiftClient.Application.Enums;

namespace BBLiftClient.Application.Dtos;

public record UpdateOrderRequest(
    Guid Id,
    OrderStatus Status
);