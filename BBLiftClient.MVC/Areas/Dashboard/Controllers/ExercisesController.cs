using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BBLiftClient.MVC.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class ExercisesController(
    ExerciseService service,
    MuscleGroupService muscleGroupService,
    EquipmentService equipmentService)
    : Controller
{
    private async Task LoadListsAsync(Guid? muscleGroupId = null, Guid? equipmentId = null)
    {
        ViewBag.MuscleGroups = new SelectList(await muscleGroupService.GetAllAsync(), "Id", "Name", muscleGroupId);
        ViewBag.Equipments = new SelectList(await equipmentService.GetAllAsync(), "Id", "Name", equipmentId);
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.MuscleNames = (await muscleGroupService.GetAllAsync()).ToDictionary(m => m.Id, m => m.Name);
        ViewBag.EquipmentNames = (await equipmentService.GetAllAsync()).ToDictionary(e => e.Id, e => e.Name);

        return View(await service.GetAllAsync());
    }

    public async Task<IActionResult> Create()
    {
        await LoadListsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExerciseRequest request)
    {
        ModelState.Clear();

        request = request with
        {
            Name = request.Name?.Trim() ?? string.Empty,
            Description = request.Description?.Trim() ?? string.Empty
        };

        var response = await service.CreateAsync(request);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Egzersiz eklendi.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var item in response.ErrorMessage.Split(", ", StringSplitOptions.RemoveEmptyEntries))
        {
            ModelState.AddModelError(string.Empty, item);
        }

        await LoadListsAsync(request.MuscleGroupId, request.EquipmentId);
        return View(request);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await service.GetByIdAsync(id);

        if (response is null) return NotFound();

        await LoadListsAsync(response.MuscleGroupId, response.EquipmentId);

        return View(
            new UpdateExerciseRequest(
                response.Id,
                response.Name,
                response.Description,
                response.MuscleGroupId,
                response.EquipmentId
                )
            );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateExerciseRequest request)
    {
        ModelState.Clear();

        request = request with
        {
            Name = request.Name?.Trim() ?? string.Empty,
            Description = request.Description?.Trim() ?? string.Empty
        };

        var response = await service.UpdateAsync(id, request);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Egzersiz güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var item in response.ErrorMessage.Split(", ", StringSplitOptions.RemoveEmptyEntries))
        {
            ModelState.AddModelError(string.Empty, item);
        }

        await LoadListsAsync(request.MuscleGroupId, request.EquipmentId);
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await service.DeleteAsync(id);

        if (response.IsSuccess)
            TempData["SuccessMessage"] = "Egzersiz silindi.";
        else
            TempData["ErrorMessage"] = response.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }
}