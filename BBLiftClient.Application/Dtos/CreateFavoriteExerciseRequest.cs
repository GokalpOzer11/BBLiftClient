namespace BBLiftClient.Application.Dtos;

public record CreateFavoriteExerciseRequest(
    string UserId,
    Guid ExerciseId
);