namespace BBLiftClient.Application.Dtos;

public record RecentWorkoutResponse(
    Guid Id,
    string Name,
    DateTimeOffset Date,
    int DurationInMinutes,
    decimal TotalVolume
);