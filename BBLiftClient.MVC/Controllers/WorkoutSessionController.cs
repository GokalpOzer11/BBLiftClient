using BBLift.Application.Dtos;
using BBLiftClient.Application.Dtos; 
using BBLiftClient.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BBLiftClient.Application.Services;

namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class WorkoutSessionController : Controller
{
    private readonly WorkoutSessionService _workoutSessionService;
    private readonly ExerciseService _exerciseService;
    private readonly WorkoutLogService _workoutLogService;
    private readonly RoutineService _routineService;

    public WorkoutSessionController(
        WorkoutSessionService workoutSessionService, 
        ExerciseService exerciseService,
        WorkoutLogService workoutLogService,
        RoutineService routineService)
    {
        _workoutSessionService = workoutSessionService;
        _exerciseService = exerciseService;
        _workoutLogService = workoutLogService;
        _routineService = routineService;
    }


    private string? GetUserId() =>
        User.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

    private async Task<string?> ResolveRoutineNameAsync(Guid? routineId)
    {
        if (!routineId.HasValue || routineId.Value == Guid.Empty) return null;

        var routine = await _routineService.GetByIdAsync(routineId.Value);
        return routine?.Name;
    }

    private async Task<List<WorkoutSessionResponse>> GetUserSessionsAsync(string userId)
    {
        var sessions = await _workoutSessionService.GetAllByUserIdAsync(userId);
        return sessions?.OrderByDescending(x => x.StartTime).ToList() ?? new List<WorkoutSessionResponse>();
    }

    private async Task<IActionResult> ShowActiveAsync(
        Guid sessionId,
        string sessionName,
        string? routineName,
        List<PrefillExercise>? prefill = null)
    {
        ViewBag.SessionId = sessionId;
        ViewBag.RoutineName = routineName;
        ViewBag.SessionName = sessionName;
        ViewBag.Exercises = await _exerciseService.GetAllAsync() ?? new List<ExerciseResponse>();
        ViewBag.Prefill = prefill ?? new List<PrefillExercise>();
        return View("Active");
    }

    private async Task<IActionResult> ShowExistingActiveAsync(WorkoutSessionResponse activeSession)
    {
        var routineName = await ResolveRoutineNameAsync(activeSession.RoutineId);

        TempData["InfoMessage"] = "Antrenman devam ediyor! Başka bir antrenman başlatmadan önce bunu bitirmelisiniz.";

        var sessionName = !string.IsNullOrWhiteSpace(activeSession.Name)
            ? activeSession.Name
            : (routineName ?? (activeSession.RoutineId.HasValue ? "Planlı Rutin" : "Serbest Antrenman"));

        return await ShowActiveAsync(activeSession.Id, sessionName, routineName);
    }

    private static List<PrefillExercise> ToPrefill(RoutineResponse? routine)
    {
        if (routine?.Exercises == null)
            return new List<PrefillExercise>();

        return routine.Exercises
            .Where(re => re.ExerciseId != Guid.Empty)
            .Select(re => new PrefillExercise(
                re.ExerciseId,
                re.ExerciseName,
                Enumerable.Range(1, Math.Max(1, re.Sets))
                    .Select(n => new PrefillSet(n, re.Weight, re.Reps, null))
                    .ToList()))
            .ToList();
    }

    private async Task<List<PrefillExercise>> BuildPrefillAsync(Guid sourceSessionId, Guid? routineId)
    {
        var logs = (await _workoutLogService.GetBySessionIdAsync(sourceSessionId)).ToList();

        if (logs.Count > 0)
        {
            return logs
                .GroupBy(l => l.ExerciseId)
                .Select(g => new PrefillExercise(
                    g.Key,
                    g.First().ExerciseName ?? "Egzersiz",
                    g.OrderBy(l => l.SetNumber)
                        .Select(l => new PrefillSet(l.SetNumber, l.Weight, l.Reps, l.Rir))
                        .ToList()))
                .ToList();
        }

        if (routineId.HasValue && routineId.Value != Guid.Empty)
        {
            var routine = await _routineService.GetByIdAsync(routineId.Value);
            return ToPrefill(routine);
        }

        return new List<PrefillExercise>();
    }


    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> Active(Guid routineId)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Content("HATA: Kullanıcı bulunamadı.");

        var userSessions = await GetUserSessionsAsync(userId);
        var activeSession = userSessions.FirstOrDefault(x => x.EndTime == null);

        if (activeSession != null)
            return await ShowExistingActiveAsync(activeSession);

        var request = new CreateWorkoutSessionRequest(userId, routineId, BBLiftClient.Application.Enums.WorkoutStatus.Active);
        var sessionId = await _workoutSessionService.CreateAndReturnIdAsync(request);
        
        if (sessionId == null || sessionId == Guid.Empty) return RedirectToAction("Index", "Routine");

        var routine = await _routineService.GetByIdAsync(routineId);
        var routineName = routine?.Name;

        return await ShowActiveAsync(
            sessionId.Value,
            routineName ?? "Planlı Rutin",
            routineName,
            ToPrefill(routine));
    }
    
    [HttpGet]
    public async Task<IActionResult> StartQuick()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Content("HATA: Kullanıcı bulunamadı.");

        var userSessions = await GetUserSessionsAsync(userId);
        var activeSession = userSessions.FirstOrDefault(x => x.EndTime == null);

        if (activeSession != null)
            return await ShowExistingActiveAsync(activeSession);

        var request = new CreateWorkoutSessionRequest(userId, null, BBLiftClient.Application.Enums.WorkoutStatus.Active);
        var sessionId = await _workoutSessionService.CreateAndReturnIdAsync(request);
        
        if (sessionId == null || sessionId == Guid.Empty) return Content("HATA: Seans oluşturulamadı.");

        return await ShowActiveAsync(sessionId.Value, "Serbest Antrenman", null);
    }

    [HttpGet]
    public async Task<IActionResult> Restart(Guid id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var userSessions = await GetUserSessionsAsync(userId);

        var source = userSessions.FirstOrDefault(x => x.Id == id);
        if (source == null) return RedirectToAction("Workouts", "Account");

        var activeSession = userSessions.FirstOrDefault(x => x.EndTime == null);
        if (activeSession != null)
            return await ShowExistingActiveAsync(activeSession);

        var request = new CreateWorkoutSessionRequest(userId, source.RoutineId, BBLiftClient.Application.Enums.WorkoutStatus.Active);
        var sessionId = await _workoutSessionService.CreateAndReturnIdAsync(request);

        if (sessionId == null || sessionId == Guid.Empty) return RedirectToAction("Workouts", "Account");

        var routineName = await ResolveRoutineNameAsync(source.RoutineId);
        var prefill = await BuildPrefillAsync(source.Id, source.RoutineId);
        var sessionName = routineName ?? (source.RoutineId.HasValue ? "Planlı Rutin" : "Serbest Antrenman");

        return await ShowActiveAsync(sessionId.Value, sessionName, routineName, prefill);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSession([FromBody] UpdateWorkoutSessionRequest request)
    {
        var result = await _workoutSessionService.UpdateAsync(request);
        
        return result.IsSuccess 
            ? Json(new { success = true }) 
            : Json(new { success = false, message = result.ErrorMessage });
    }

    [HttpPost]
    public async Task<IActionResult> SaveSetLog([FromBody] CreateWorkoutLogRequest request)
    {
        var result = await _workoutLogService.CreateAsync(request);
        return result.IsSuccess 
            ? Json(new { success = true, message = "Set başarıyla kaydedildi." }) 
            : Json(new { success = false, message = result.ErrorMessage });
    }

    [HttpPost]
    public async Task<IActionResult> Finish(Guid sessionId)
    {
        await _workoutSessionService.CompleteSessionAsync(sessionId);
        return RedirectToAction("Workouts", "Account");
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var session = await _workoutSessionService.GetByIdAsync(id);
        if (session == null)
        {
            return RedirectToAction("Workouts", "Account");
        }

        var logs = await _workoutLogService.GetBySessionIdAsync(id);

        var groupedLogs = logs
            .GroupBy(l => l.ExerciseName ?? "Bilinmeyen Egzersiz")
            .ToList();

        ViewBag.GroupedLogs = groupedLogs;

        return View(session);
    }
}