namespace BBLiftClient.Application.Dtos;

public record UpdateRoutineExerciseRequest
(
    Guid RoutineId,
    Guid ExerciseId,
    int Sets,
    decimal Weight,
    int Reps,
    int? RestTimeInSeconds
);