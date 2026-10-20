namespace BBLiftClient.Application.Dtos;

public record SignUpRequest(
    string FirstName,
    string LastName,
    string UserName,
    DateOnly DateOfBirth,
    string Email,
    string Password
);