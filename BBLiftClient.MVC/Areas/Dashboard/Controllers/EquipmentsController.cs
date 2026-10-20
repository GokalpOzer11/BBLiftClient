using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class EquipmentsController(EquipmentService service)
    : Controller
{
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEquipmentRequest request)
    {
        ModelState.Clear();

        request = request with { Name = request.Name?.Trim() ?? string.Empty };

        var response = await service.CreateAsync(request);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Ekipman eklendi.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var item in response.ErrorMessage.Split(", ", StringSplitOptions.RemoveEmptyEntries))
        {
            ModelState.AddModelError(string.Empty, item);
        }

        return View(request);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await service.GetByIdAsync(id);

        if (response is null) return NotFound();

        return View(new UpdateEquipmentRequest(response.Id, response.Name));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateEquipmentRequest request)
    {
        ModelState.Clear();

        request = request with { Name = request.Name?.Trim() ?? string.Empty };

        var response = await service.UpdateAsync(id, request);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Ekipman güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var item in response.ErrorMessage.Split(", ", StringSplitOptions.RemoveEmptyEntries))
        {
            ModelState.AddModelError(string.Empty, item);
        }

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await service.DeleteAsync(id);

        if (response.IsSuccess)
            TempData["SuccessMessage"] = "Ekipman silindi.";
        else
            TempData["ErrorMessage"] = response.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }
}