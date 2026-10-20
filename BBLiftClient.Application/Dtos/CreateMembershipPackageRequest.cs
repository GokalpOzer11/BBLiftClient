namespace BBLiftClient.Application.Dtos;

public record CreateMembershipPackageRequest
(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int DurationInMonths
);