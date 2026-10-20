using System.Security.Claims;
using System.Text.Json;
using BBLiftClient.Application.Dtos;
using BBLiftClient.Application.Services;
using BBLiftClient.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class RoutineController : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly RoutineService _routineService;
    private readonly RoutineExerciseService _routineExerciseService;
    private readonly ExerciseService _exerciseService;

    public RoutineController(
        RoutineService routineService,
        RoutineExerciseService routineExerciseService,
        ExerciseService exerciseService)
    {
        _routineService = routineService;
        _routineExerciseService = routineExerciseService;
        _exerciseService = exerciseService;
    }

    private string? GetUserId() =>
        User.FindFirst("nameid")?.Value
        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private async Task LoadExercisesAsync()
    {
        ViewBag.Exercises = await _exerciseService.GetAllAsync() ?? new List<ExerciseResponse>();
    }

    private static List<RoutineExerciseInput>? ParseExercises(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<RoutineExerciseInput>();

        try
        {
            return JsonSerializer.Deserialize<List<RoutineExerciseInput>>(json, JsonOptions)
                   ?? new List<RoutineExerciseInput>();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // ---------- Liste ----------

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var routines = await _routineService.GetAllByUserIdAsync(userId);
        return View(routines ?? new List<RoutineResponse>());
    }

    // ---------- Oluştur (rutin + egzersizler tek sayfada) ----------

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadExercisesAsync();
        return View(new CreateRoutineViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoutineViewModel model)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var exercises = ParseExercises(model.ExercisesJson);

        if (exercises == null)
        {
            ModelState.AddModelError(string.Empty, "Egzersiz listesi okunamadı, lütfen tekrar deneyin.");
        }
        else if (exercises.Any(e => e.Sets < 1 || e.Reps < 1 || e.Weight < 0 || e.RestTimeInSeconds < 0))
        {
            ModelState.AddModelError(string.Empty,
                "Set ve tekrar en az 1 olmalı; ağırlık ve dinlenme süresi negatif olamaz.");
        }

        if (!ModelState.IsValid)
        {
            await LoadExercisesAsync();
            return View(model);
        }

        var routineResult = await _routineService.CreateAsync(
            new CreateRoutineRequest(userId, model.Name.Trim()));

        if (!routineResult.IsSuccess || routineResult.Data == null)
        {
            ModelState.AddModelError(string.Empty, routineResult.ErrorMessage ?? "Rutin oluşturulamadı.");
            await LoadExercisesAsync();
            return View(model);
        }

        var routineId = routineResult.Data.Id;

        // Sıra korunsun diye egzersizleri tek tek, sırayla ekliyoruz
        var errors = new List<string>();
        foreach (var e in exercises!)
        {
            var addResult = await _routineExerciseService.CreateAsync(
                new CreateRoutineExerciseRequest(
                    routineId, e.ExerciseId, e.Sets, e.Weight, e.Reps, e.RestTimeInSeconds));

            if (!addResult.IsSuccess)
                errors.Add(addResult.ErrorMessage);
        }

        if (errors.Count > 0)
        {
            TempData["ErrorMessage"] =
                $"Rutin oluşturuldu ancak {errors.Count} egzersiz eklenemedi ({errors[0]}). Buradan tekrar ekleyebilirsin.";
            return RedirectToAction(nameof(Detail), new { id = routineId });
        }

        TempData["SuccessMessage"] = "Yeni rutin başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Sil ----------

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _routineService.DeleteAsync(id);

        if (result.IsSuccess)
            TempData["SuccessMessage"] = "Rutin başarıyla silindi.";
        else
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    // ---------- Detay / Düzenleme ----------

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var routine = await _routineService.GetByIdAsync(id);
        if (routine == null)
        {
            TempData["ErrorMessage"] = "Rutin bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        await LoadExercisesAsync();
        return View(routine);
    }

    [HttpPost]
    public async Task<IActionResult> AddExercise([FromBody] CreateRoutineExerciseRequest request)
    {
        var result = await _routineExerciseService.CreateAsync(request);

        if (result.IsSuccess)
        {
            return Json(new { success = true, id = result.Data, message = "Egzersiz rutine başarıyla eklendi." });
        }

        return Json(new { success = false, message = result.ErrorMessage });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateExercise([FromBody] UpdateRoutineExerciseInput input)
    {
        if (input.ExerciseId == Guid.Empty)
            return Json(new { success = false, message = "Egzersiz eşleştirilemedi." });

        if (input.Sets < 1 || input.Reps < 1 || input.Weight < 0 || input.RestTimeInSeconds < 0)
            return Json(new
            {
                success = false,
                message = "Set ve tekrar en az 1 olmalı; ağırlık ve dinlenme negatif olamaz."
            });

        var result = await _routineExerciseService.UpdateAsync(
            input.Id,
            new UpdateRoutineExerciseRequest(
                input.RoutineId, input.ExerciseId, input.Sets, input.Weight, input.Reps, input.RestTimeInSeconds));

        if (result.IsSuccess)
        {
            return Json(new { success = true, message = "Egzersiz güncellendi." });
        }

        return Json(new { success = false, message = result.ErrorMessage });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveExercise(Guid id)
    {
        var result = await _routineExerciseService.DeleteAsync(id);

        if (result.IsSuccess)
        {
            return Json(new { success = true, message = "Egzersiz rutinden çıkarıldı." });
        }

        return Json(new { success = false, message = result.ErrorMessage });
    }
}