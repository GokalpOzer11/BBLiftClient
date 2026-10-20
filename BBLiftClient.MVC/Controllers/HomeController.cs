using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class HomeController(
    ExerciseService exerciseService,
    MuscleGroupService muscleGroupService,
    MembershipPackageService packageService)
    : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            ViewBag.ExerciseCount = (await exerciseService.GetAllAsync()).Count();
            ViewBag.MuscleGroupCount = (await muscleGroupService.GetAllAsync()).Count();

            var packages = (await packageService.GetAllAsync()).OrderBy(p => p.Price).ToList();
            ViewBag.PackageCount = packages.Count;
            ViewBag.Packages = packages.Take(3).ToList();
        }
        catch (HttpRequestException)
        {
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}