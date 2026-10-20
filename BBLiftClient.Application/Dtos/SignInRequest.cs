namespace BBLiftClient.Application.Dtos;

public record SignInRequest(
    string Email,
    string Password
);