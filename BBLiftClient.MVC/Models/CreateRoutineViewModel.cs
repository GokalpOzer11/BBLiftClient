using System.ComponentModel.DataAnnotations;

namespace BBLiftClient.MVC.Models;

public class CreateRoutineViewModel
{
    [Required(ErrorMessage = "Rutin adı boş olamaz.")]
    [StringLength(100, ErrorMessage = "Rutin adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    public string? ExercisesJson { get; set; }
}

public record RoutineExerciseInput(
    Guid ExerciseId,
    int Sets,
    decimal Weight,
    int Reps,
    int? RestTimeInSeconds
);

public record UpdateRoutineExerciseInput(
    Guid Id,
    Guid RoutineId,
    Guid ExerciseId,
    int Sets,
    decimal Weight,
    int Reps,
    int? RestTimeInSeconds
);