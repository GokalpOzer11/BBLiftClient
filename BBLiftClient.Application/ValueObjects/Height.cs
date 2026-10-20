using BBLiftClient.Application.Enums;

namespace BBLift.Domain.ValueObjects;

public record Height
{
    public double? Value { get; set; }
    public HeightUnit? Unit { get; set; } 
    
    private Height(double? value, HeightUnit? unit)
    {
        Value = value;
        Unit = unit;
    }
    
    public static Height Create(double? value, HeightUnit? unit)
    {
        if (value.HasValue && value.Value <= 0)
            throw new ArgumentException("Uzunluk 0 dan Büyük Olmalı", nameof(value));

        if (value.HasValue && unit == HeightUnit.Cm && value.Value > 300)
            throw new ArgumentException("Tanımsız Uzunluk Değeri", nameof(value));

        return new Height(value, unit);
    }
    
    public Height ToCm()
    {
        if (Unit == HeightUnit.Cm || !Value.HasValue) return this;
        return new Height(Value.Value * 30.48, HeightUnit.Cm);
    }
    
    public Height ToFt()
    {
        if (Unit == HeightUnit.Ft || !Value.HasValue) return this;
        return new Height(Value.Value / 30.48, HeightUnit.Ft);
    }
    
    public override string ToString() => Value.HasValue && Unit.HasValue ? $"{Value.Value:N2} {Unit.Value}" : "";
}