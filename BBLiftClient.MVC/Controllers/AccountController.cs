using BBLiftClient.Application.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace BBLiftClient.MVC.Controllers;

public class AccountController(
    AuthService service,
    WorkoutSessionService workoutSessionService,
    RoutineService routineService)
    : Controller
{
    private string? GetUserId() =>
        User.FindFirst("nameid")?.Value
        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(SignUpRequest request)
    {
        var response = await service.RegisterAsync(request);
        
        if (response.IsSuccess) 
            return RedirectToAction(nameof(Login));

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            foreach (var item in response.ErrorMessage.Split(","))
            {
                ModelState.AddModelError(string.Empty, item);
            }
        }

        return View(request);
    }

    public IActionResult Login(string? returnUrl)
    {
        if (returnUrl is not null)
            TempData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(SignInRequest request)
    {
        var response = await service.LoginAsync(request);
        
        if (response.IsSuccess && response.Data != null)
        {
            await SignInAsync(response.Data);

            var returnUrl = TempData["ReturnUrl"];
            if (returnUrl is not null)
                if (!string.IsNullOrEmpty(returnUrl.ToString()) && Url.IsLocalUrl(returnUrl.ToString()))
                    return Redirect(returnUrl.ToString() ?? "/");

            return RedirectToAction("Index", "Home");
        }

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            foreach (var item in response.ErrorMessage.Split(","))
            {
                ModelState.AddModelError(string.Empty, item);
            }
        }

        return View(request);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    private async Task SignInAsync(TokenModel model)
    {
        List<Claim> claims = [];

        foreach (var item in model.Claims)
        {
            claims.Add(new(item.Type, item.Value));
        }

        claims.Add(new("jwt_token", model.AccessToken));

        var identity = new ClaimsIdentity(
            claims, 
            CookieAuthenticationDefaults.AuthenticationScheme, 
            "name",
            "role"
            
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            });
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var response = await service.GetProfileAsync();
        
        if (!response.IsSuccess || response.Data == null)
            return RedirectToAction("Index", "Home"); 

        var routines = new List<RoutineResponse>();
        var workoutTitles = new Dictionary<Guid, string>();

        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            routines = (await routineService.GetAllByUserIdAsync(userId)).ToList();
            var routineNames = routines.ToDictionary(r => r.Id, r => r.Name);

            var sessions = await workoutSessionService.GetAllByUserIdAsync(userId);
            foreach (var session in sessions)
            {
                if (session.RoutineId.HasValue
                    && routineNames.TryGetValue(session.RoutineId.Value, out var routineName))
                {
                    workoutTitles[session.Id] = routineName;
                }
            }
        }

        ViewBag.Routines = routines;
        ViewBag.WorkoutTitles = workoutTitles;

        return View(response.Data); 
    }

    [Authorize]
    public async Task<IActionResult> Settings()
    {
        var response = await service.GetProfileAsync();
        
        if (!response.IsSuccess || response.Data == null)
            return RedirectToAction("Index", "Home"); 

        ViewBag.Profile = response.Data;
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var response = await service.UpdateProfileAsync(request);
        
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Profil bilgileri başarıyla güncellendi.";
            return RedirectToAction(nameof(Profile));
        }

        if (!string.IsNullOrEmpty(response.ErrorMessage))
            ModelState.AddModelError(string.Empty, response.ErrorMessage);

        var profileData = await service.GetProfileAsync();
        ViewBag.Profile = profileData.Data;
        return View("Settings", request);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var response = await service.ChangePasswordAsync(request);
        
        if (response.IsSuccess)
        {
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction(nameof(Profile));
        }

        if (!string.IsNullOrEmpty(response.ErrorMessage))
            ModelState.AddModelError(string.Empty, response.ErrorMessage);

        var profileData = await service.GetProfileAsync();
        ViewBag.Profile = profileData.Data;
        return View("Settings"); 
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Workouts()
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction(nameof(Login));
        }

        var sessions = await workoutSessionService.GetAllByUserIdAsync(userId);

        var history = sessions?.OrderByDescending(x => x.StartTime).ToList() ?? new();

        var routines = await routineService.GetAllByUserIdAsync(userId);
        ViewBag.RoutineNames = routines.ToDictionary(r => r.Id, r => r.Name);

        return View(history);
    }
}