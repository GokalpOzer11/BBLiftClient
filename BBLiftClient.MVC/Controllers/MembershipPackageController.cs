using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BBLiftClient.Application.Services;


namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class MembershipPackageController : Controller
{
    private readonly MembershipPackageService _membershipPackageService;

    public MembershipPackageController(MembershipPackageService membershipPackageService)
    {
        _membershipPackageService = membershipPackageService;
    }

    public async Task<IActionResult> Index()
    {
       
        var packages = await _membershipPackageService.GetAllAsync(); 
        
        return View(packages);
    }
}