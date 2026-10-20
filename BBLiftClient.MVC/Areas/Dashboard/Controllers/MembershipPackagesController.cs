using System.Globalization;
using BBLiftClient.Application.Common;
using BBLiftClient.Application.Dtos;
using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class MembershipPackagesController(MembershipPackageService service)
    : Controller
{
    // Tarayıcı sayıyı her zaman "." ile gönderir; sunucunun kültüründen etkilenmesin diye elle okuyoruz
    private decimal ReadPrice(decimal fallback)
    {
        var raw = Request.Form["Price"].ToString().Replace(',', '.');

        return decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var price)
            ? price
            : fallback;
    }

    public async Task<IActionResult> Index() => View(await service.GetAllAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMembershipPackageRequest request)
    {
        ModelState.Clear();

        request = request with
        {
            Name = request.Name?.Trim() ?? string.Empty,
            Description = request.Description?.Trim() ?? string.Empty,
            Price = ReadPrice(request.Price),
            Currency = "TRY"
        };

        var response = await service.CreateAsync(request);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Üyelik paketi eklendi.";
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

        return View(
            new UpdateMembershipPackageRequest(
                response.Id,
                response.Name,
                response.Description,
                response.Price,
                response.Currency,
                response.DurationInMonths
                )
            );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateMembershipPackageRequest request)
    {
        ModelState.Clear();

        request = request with
        {
            Name = request.Name?.Trim() ?? string.Empty,
            Description = request.Description?.Trim() ?? string.Empty,
            Price = ReadPrice(request.Price),
            Currency = "TRY"
        };

        var response = await service.UpdateAsync(id, request);

        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Üyelik paketi güncellendi.";
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
            TempData["SuccessMessage"] = "Üyelik paketi silindi.";
        else
            TempData["ErrorMessage"] = response.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }
}