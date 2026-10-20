namespace BBLiftClient.Application.Dtos;

public record CreateOrderRequest(
    string UserId,
    Guid MembershipPackageId,
    decimal TotalAmount
);