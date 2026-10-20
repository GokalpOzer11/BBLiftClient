namespace BBLiftClient.Application.Dtos;

public record MembershipPackageResponse
(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int DurationInMonths
);