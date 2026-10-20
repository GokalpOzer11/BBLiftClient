using System.Security.Claims;
using BBLiftClient.Application.Dtos;
using BBLiftClient.Application.Services;
using BBLiftClient.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class CartController(
    CartService cartService,
    MembershipPackageService packageService)
    : Controller
{
    private string? GetUserId() =>
        User.FindFirst("nameid")?.Value
        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var model = await CartViewBuilder.BuildAsync(userId, cartService, packageService);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(Guid packageId)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var cart = await cartService.GetByUserIdAsync(userId);

        BBLiftClient.Application.Common.Result result;

        if (cart is null)
        {
            result = await cartService.CreateAsync(new CreateCartRequest(userId, packageId, 1));
        }
        else
        {
            var existing = cart.Items.FirstOrDefault(i => i.MembershipPackageId == packageId);

            result = existing is null
                ? await cartService.AddItemAsync(new CreateCartItemRequest(cart.Id, packageId, 1))
                : await cartService.UpdateItemAsync(new UpdateCartItemRequest(existing.Id, existing.Quantity + 1));
        }

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index", "MembershipPackage");
        }

        TempData["SuccessMessage"] = "Paket sepete eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(Guid itemId, int quantity)
    {
        var result = quantity < 1
            ? await cartService.RemoveItemAsync(itemId)
            : await cartService.UpdateItemAsync(new UpdateCartItemRequest(itemId, quantity));

        if (!result.IsSuccess)
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid itemId)
    {
        var result = await cartService.RemoveItemAsync(itemId);

        if (result.IsSuccess)
            TempData["SuccessMessage"] = "Ürün sepetten çıkarıldı.";
        else
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        await cartService.ClearAsync(userId);

        TempData["SuccessMessage"] = "Sepet temizlendi.";
        return RedirectToAction(nameof(Index));
    }
}