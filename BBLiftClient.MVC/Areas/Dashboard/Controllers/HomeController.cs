using BBLiftClient.Application.Common;
using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class HomeController(
    ExerciseService exerciseService,
    MuscleGroupService muscleGroupService,
    EquipmentService equipmentService,
    MembershipPackageService packageService)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.ExerciseCount = (await exerciseService.GetAllAsync()).Count();
        ViewBag.MuscleGroupCount = (await muscleGroupService.GetAllAsync()).Count();
        ViewBag.EquipmentCount = (await equipmentService.GetAllAsync()).Count();
        ViewBag.PackageCount = (await packageService.GetAllAsync()).Count();

        return View();
    }
}