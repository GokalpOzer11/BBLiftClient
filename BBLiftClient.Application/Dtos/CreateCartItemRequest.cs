namespace BBLiftClient.Application.Dtos;

public record CreateCartItemRequest
(
    Guid CartId,
    Guid MembershipPackageId,
    int Quantity
);