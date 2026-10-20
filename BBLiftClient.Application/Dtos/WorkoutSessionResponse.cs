using BBLiftClient.Application.Enums;

namespace BBLiftClient.Application.Dtos;

public record WorkoutSessionResponse
(
    Guid Id,
    Guid? RoutineId, 
    string? Name,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime,
    int WorkOutDurationInSeconds,
    WorkoutStatus Status
);