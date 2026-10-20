using System.Security.Claims;
using BBLiftClient.Application.Dtos;
using BBLiftClient.Application.Enums;
using BBLiftClient.Application.Services;
using BBLiftClient.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBLiftClient.MVC.Controllers;

[Authorize]
public class CheckoutController(
    CartService cartService,
    MembershipPackageService packageService,
    OrderService orderService,
    PaymentService paymentService)
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

        var cart = await CartViewBuilder.BuildAsync(userId, cartService, packageService);
        if (!cart.Lines.Any())
        {
            TempData["ErrorMessage"] = "Sepetiniz boş.";
            return RedirectToAction("Index", "Cart");
        }

        ViewBag.Cart = cart;
        return View(new CheckoutForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(CheckoutForm form)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        var cart = await CartViewBuilder.BuildAsync(userId, cartService, packageService);
        if (!cart.Lines.Any())
        {
            TempData["ErrorMessage"] = "Sepetiniz boş.";
            return RedirectToAction("Index", "Cart");
        }

        ValidateExpiry(form.Expiry);

        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            return View("Index", form);
        }

        
        var failures = new List<string>();

        foreach (var line in cart.Lines)
        {
            var order = await orderService.CreateAsync(
                new CreateOrderRequest(userId, line.PackageId, line.LineTotal));

            if (!order.IsSuccess || order.Data is null)
            {
                failures.Add($"{line.PackageName}: {order.ErrorMessage}");
                continue;
            }

            var orderId = order.Data.Id;

            var payment = await paymentService.CreateAsync(
                new CreatePaymentRequest(orderId, line.LineTotal, form.Method));

            if (!payment.IsSuccess)
            {
                await orderService.UpdateStatusAsync(orderId, new UpdateOrderRequest(orderId, OrderStatus.Failed));
                failures.Add($"{line.PackageName}: {payment.ErrorMessage}");
                continue;
            }

            var paymentId = payment.Data;

            var paid = await paymentService.UpdateStatusAsync(
                paymentId, new UpdatePaymentRequest(paymentId, PaymentStatus.Completed));

            var finalStatus = paid.IsSuccess ? OrderStatus.Completed : OrderStatus.Failed;
            await orderService.UpdateStatusAsync(orderId, new UpdateOrderRequest(orderId, finalStatus));

            if (!paid.IsSuccess)
                failures.Add($"{line.PackageName}: {paid.ErrorMessage}");
        }

        if (failures.Count == 0)
            TempData["SuccessMessage"] = "Ödemeniz alındı, siparişleriniz oluşturuldu.";
        else
            TempData["ErrorMessage"] = "Bazı siparişler tamamlanamadı: " + string.Join(" | ", failures);

        return RedirectToAction("Index", "Order");
    }

    private void ValidateExpiry(string? expiry)
    {
        if (string.IsNullOrWhiteSpace(expiry) || expiry.Length != 5 || expiry[2] != '/') return;
        if (!int.TryParse(expiry[..2], out var month) || !int.TryParse(expiry[3..], out var yy)) return;
        if (month < 1 || month > 12) return;

        var year = 2000 + yy;
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        if (lastDay < DateTime.Today)
            ModelState.AddModelError(nameof(CheckoutForm.Expiry), "Kartın son kullanma tarihi geçmiş.");
    }
}