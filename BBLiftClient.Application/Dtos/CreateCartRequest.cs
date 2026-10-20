namespace BBLiftClient.Application.Dtos;

public record CreateCartRequest
(
    string UserId,
    Guid MembershipPackageId,
    int Quantity
);