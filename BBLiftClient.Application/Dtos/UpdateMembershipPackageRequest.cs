namespace BBLiftClient.Application.Dtos;

public record UpdateMembershipPackageRequest
(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int DurationInMonths
);