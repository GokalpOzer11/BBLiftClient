namespace BBLiftClient.Application.Dtos;

public record ExerciseResponse(
    Guid Id,
    string Name,
    string Description,
    Guid MuscleGroupId,
    Guid EquipmentId
);