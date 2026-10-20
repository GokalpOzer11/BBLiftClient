using System.Globalization;
using BBLiftClient.Application.Dtos;
using BBLiftClient.Application.Services;
using BBLiftClient.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class ExerciseController(
    ExerciseService exerciseService,
    EquipmentService equipmentService,
    MuscleGroupService muscleGroupService)
    : Controller
{
    private const int PageSize = 12;
    private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

    private static bool Has(string? text, string term) =>
        !string.IsNullOrEmpty(text) && Tr.CompareInfo.IndexOf(text, term, CompareOptions.IgnoreCase) >= 0;

    public async Task<IActionResult> Index(string? q, Guid? muscleGroupId, Guid? equipmentId, int page = 1)
    {
        var all = (await exerciseService.GetAllAsync()).ToList();
        var muscleGroups = (await muscleGroupService.GetAllAsync()).OrderBy(m => m.Name).ToList();
        var equipments = (await equipmentService.GetAllAsync()).OrderBy(e => e.Name).ToList();

        var muscleNames = muscleGroups.ToDictionary(m => m.Id, m => m.Name);
        var equipmentNames = equipments.ToDictionary(e => e.Id, e => e.Name);

        IEnumerable<ExerciseResponse> filtered = all;

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            filtered = filtered.Where(e =>
                Has(e.Name, term)
                || Has(e.Description, term)
                || (muscleNames.TryGetValue(e.MuscleGroupId, out var muscle) && Has(muscle, term))
                || (equipmentNames.TryGetValue(e.EquipmentId, out var equipment) && Has(equipment, term)));
        }

        if (muscleGroupId.HasValue)
            filtered = filtered.Where(e => e.MuscleGroupId == muscleGroupId.Value);

        if (equipmentId.HasValue)
            filtered = filtered.Where(e => e.EquipmentId == equipmentId.Value);

        var list = filtered.OrderBy(e => e.Name).ToList();

        var totalPages = Math.Max(1, (int)Math.Ceiling(list.Count / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = list.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        var model = new ExerciseListViewModel(
            items,
            muscleGroups,
            equipments,
            muscleNames,
            equipmentNames,
            q?.Trim(),
            muscleGroupId,
            equipmentId,
            page,
            totalPages,
            list.Count,
            all.Count);

        return View(model);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        if (id == Guid.Empty)
        {
            return RedirectToAction(nameof(Index));
        }

        var exercise = await exerciseService.GetByIdAsync(id);
        if (exercise == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var muscleGroup = await muscleGroupService.GetByIdAsync(exercise.MuscleGroupId);
        var equipment = await equipmentService.GetByIdAsync(exercise.EquipmentId);

        ViewBag.MuscleName = muscleGroup?.Name ?? "-";
        ViewBag.MuscleImage = muscleGroup?.GifUrl;
        ViewBag.EquipmentName = equipment?.Name ?? "-";

        return View(exercise);
    }
}