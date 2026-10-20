using BBLiftClient.Application.Enums;

namespace BBLift.Application.Dtos;

public record UpdateWorkoutSessionRequest
(
    Guid Id,
    
    string? Name,
    DateTimeOffset? EndTime,
    int WorkOutDurationInSeconds,
    WorkoutStatus Status
);