namespace BBLiftClient.Application.Dtos;

public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string Email,
    decimal Weight,
    decimal Height
);