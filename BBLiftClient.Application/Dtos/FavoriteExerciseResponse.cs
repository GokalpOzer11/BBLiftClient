namespace BBLiftClient.Application.Dtos;

public record FavoriteExerciseResponse(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName
);