namespace BBLiftClient.Application.Dtos;

public record CreateExerciseRequest(
    string Name,
    string Description,
    Guid MuscleGroupId,
    Guid EquipmentId
);