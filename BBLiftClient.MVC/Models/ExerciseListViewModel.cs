using BBLiftClient.Application.Dtos;

namespace BBLiftClient.MVC.Models;

public record ExerciseListViewModel(
    IReadOnlyList<ExerciseResponse> Items,
    IReadOnlyList<MuscleGroupResponse> MuscleGroups,
    IReadOnlyList<EquipmentResponse> Equipments,
    IReadOnlyDictionary<Guid, string> MuscleNames,
    IReadOnlyDictionary<Guid, string> EquipmentNames,
    string? Query,
    Guid? MuscleGroupId,
    Guid? EquipmentId,
    int Page,
    int TotalPages,
    int TotalCount,
    int AllCount
);