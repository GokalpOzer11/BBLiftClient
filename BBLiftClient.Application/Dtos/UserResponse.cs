namespace BBLiftClient.Application.Dtos;

public record UserResponse
(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles
);