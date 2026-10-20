namespace BBLiftClient.Application.Dtos;

public record CreateMuscleGroupRequest(
    string Name, 
    string? ImageUrl
);