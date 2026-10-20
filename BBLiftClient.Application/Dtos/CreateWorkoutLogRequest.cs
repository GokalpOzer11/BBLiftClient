using BBLiftClient.Application.Enums;

namespace BBLiftClient.Application.Dtos;

public record CreateWorkoutLogRequest
(
    Guid WorkoutSessionId,
    Guid ExerciseId,
    int SetNumber,
    decimal Weight,
    int Reps,
    SetStatus Status,
    int? Rir,
    int? RestTimeInSeconds
);