namespace BBLiftClient.Application.Dtos;

public record TokenModel(
    string AccessToken,
    string RefreshToken,
    List<ClaimModel> Claims
);