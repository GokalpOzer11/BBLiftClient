namespace BBLiftClient.Application.Dtos;

public record UpdateRoutineRequest
(
    string Name,
    Guid RoutineId
);