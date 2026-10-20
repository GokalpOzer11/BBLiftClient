using BBLiftClient.Application.Enums;

namespace BBLiftClient.Application.Dtos;

public record CreateWorkoutSessionRequest(
    string UserId,
    Guid? RoutineId,
    WorkoutStatus Status
);