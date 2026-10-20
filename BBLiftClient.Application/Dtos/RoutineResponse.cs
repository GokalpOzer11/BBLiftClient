namespace BBLiftClient.Application.Dtos;

public record RoutineResponse
(
    Guid Id,
    string Name,
    IEnumerable<RoutineExerciseResponse> Exercises
);