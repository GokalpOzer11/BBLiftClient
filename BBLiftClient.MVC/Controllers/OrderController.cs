using System.Security.Claims;
using BBLiftClient.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class OrderController(OrderService orderService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst("nameid")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var orders = await orderService.GetAllByUserIdAsync(userId);
        return View(orders.ToList());
    }
}