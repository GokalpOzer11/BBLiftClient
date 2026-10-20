namespace BBLiftClient.Application.Dtos;

public record RoutineExerciseResponse
(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    int Sets,
    decimal Weight,
    int Reps,
    int? RestTimeInSeconds
);