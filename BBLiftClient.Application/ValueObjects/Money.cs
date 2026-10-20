namespace BBLiftClient.Application.ValueObjects;

public record Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Tutar negatif olamaz.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("Para birimi 3 haneli ISO kodu olmalıdır.");

        return new(amount, currency);
    }

    public static Money Zero() => new(0, "TRY");
    public static Money TRY(decimal amount) => new(amount, "TRY");

    public static Money operator +(Money left, Money right)
    {
        CheckCurrencyMatch(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        CheckCurrencyMatch(left, right);
        if (left.Amount - right.Amount < 0)
            throw new InvalidOperationException("Çıkarma işlemi sonucunda bakiye negatif olamaz.");
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";

    private static void CheckCurrencyMatch(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Farklı para birimleri arasında matematiksel işlem yapılamaz ({left.Currency} ve {right.Currency}).");
    }
}