using System.ComponentModel.DataAnnotations;
using BBLiftClient.Application.Enums;
using BBLiftClient.Application.Services;

namespace BBLiftClient.MVC.Models;

public record CartLineViewModel(
    Guid ItemId,
    Guid PackageId,
    string PackageName,
    int DurationInMonths,
    decimal UnitPrice,
    int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

public record CartViewModel(IReadOnlyList<CartLineViewModel> Lines)
{
    public decimal Total => Lines.Sum(l => l.LineTotal);
    public int ItemCount => Lines.Sum(l => l.Quantity);
}

public static class CartViewBuilder
{
    public static async Task<CartViewModel> BuildAsync(
        string userId,
        CartService cartService,
        MembershipPackageService packageService)
    {
        var cart = await cartService.GetByUserIdAsync(userId);
        if (cart is null || !cart.Items.Any())
            return new CartViewModel([]);

        var packages = (await packageService.GetAllAsync()).ToDictionary(p => p.Id);

        var lines = cart.Items
            .Where(i => packages.ContainsKey(i.MembershipPackageId))
            .Select(i =>
            {
                var p = packages[i.MembershipPackageId];
                return new CartLineViewModel(i.Id, p.Id, p.Name, p.DurationInMonths, p.Price, i.Quantity);
            })
            .ToList();

        return new CartViewModel(lines);
    }
}

public class CheckoutForm
{
    [Required(ErrorMessage = "Kart üzerindeki isim zorunludur.")]
    [StringLength(100, ErrorMessage = "İsim en fazla 100 karakter olabilir.")]
    public string CardHolder { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kart numarası zorunludur.")]
    [RegularExpression(@"^(\d[ ]?){16}$", ErrorMessage = "Kart numarası 16 haneli olmalıdır.")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Son kullanma tarihi AA/YY biçiminde olmalıdır.")]
    public string Expiry { get; set; } = string.Empty;

    [Required(ErrorMessage = "CVV zorunludur.")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV 3 veya 4 haneli olmalıdır.")]
    public string Cvv { get; set; } = string.Empty;

    public PaymentMethod Method { get; set; } = PaymentMethod.CreditCard;
}