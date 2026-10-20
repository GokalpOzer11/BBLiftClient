namespace BBLiftClient.MVC.Models;

public record PrefillSet(int SetNumber, decimal Weight, int Reps, int? Rir);

public record PrefillExercise(Guid ExerciseId, string ExerciseName, List<PrefillSet> Sets);