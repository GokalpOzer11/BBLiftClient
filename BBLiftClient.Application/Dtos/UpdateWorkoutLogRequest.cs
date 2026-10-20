using BBLiftClient.Application.Enums;

namespace BBLift.Application.Dtos;

public record UpdateWorkoutLogRequest
(
    Guid Id,
    decimal Weight,
    int Reps,
    SetStatus Status,
    int? Rir,
    int? RestTimeInSeconds
);