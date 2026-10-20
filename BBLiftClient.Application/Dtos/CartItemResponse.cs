namespace BBLiftClient.Application.Dtos;

public record CartItemResponse
(
    Guid Id,
    Guid CartId,
    Guid MembershipPackageId,
    string PackageName,
    int Quantity,
    DateOnly CreatedOn
);