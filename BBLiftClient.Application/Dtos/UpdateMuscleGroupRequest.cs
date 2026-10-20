namespace BBLiftClient.Application.Dtos;

public record UpdateMuscleGroupRequest(
    Guid Id, 
    string Name, 
    string? ImageUrl
 
);