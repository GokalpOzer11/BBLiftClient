namespace BBLiftClient.Application.Dtos;

public record CreateRoutineExerciseRequest
(
    Guid RoutineId,
    Guid ExerciseId,
    int Sets,
    decimal Weight,
    int Reps,
    int? RestTimeInSeconds
);