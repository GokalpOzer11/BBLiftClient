using BBLiftClient.Application.Common;
using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(Roles = Roles.SuperAdmin)]
public class UsersController(UserService service)
    : Controller
{
    private static readonly string[] AssignableRoles = ["SuperAdmin", "Admin", "User"];

    public async Task<IActionResult> Index()
    {
        ViewBag.AllRoles = AssignableRoles;
        ViewBag.CurrentUserId = User.FindFirst("nameid")?.Value;

        var users = (await service.GetAllAsync()).ToList();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(Guid userId, string role)
    {
        var response = await service.AddToRoleAsync(userId, role);

        if (response.IsSuccess)
            TempData["SuccessMessage"] = $"{role} rolü eklendi. Değişiklik, kullanıcının bir sonraki girişinde geçerli olur.";
        else
            TempData["ErrorMessage"] = response.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(Guid userId, string role)
    {
        var response = await service.RemoveFromRoleAsync(userId, role);

        if (response.IsSuccess)
            TempData["SuccessMessage"] = $"{role} rolü kaldırıldı. Değişiklik, kullanıcının bir sonraki girişinde geçerli olur.";
        else
            TempData["ErrorMessage"] = response.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }
}