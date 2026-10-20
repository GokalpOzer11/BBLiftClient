namespace BBLiftClient.Application.Dtos;

public record UpdateExerciseRequest(
    Guid Id, 
    string Name,
    string Description,
    Guid MuscleGroupId,
    Guid EquipmentId
);