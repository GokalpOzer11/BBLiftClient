using System;
using BBLiftClient.Application.Enums;

namespace BBLift.Domain.ValueObjects;

public record Weight
{
    public double? Value { get; set; }
    public WeightUnit? Unit { get; set; } 
    
    private Weight(double? value, WeightUnit? unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Weight Create(double? value, WeightUnit? unit)
    {
        if (value.HasValue && value.Value < 0)
            throw new ArgumentException("Ağırlık Negatif Olamaz"); 
        return new Weight(value, unit);
    }

    public Weight ToKg()
    {
        if (Unit == WeightUnit.Kg || !Value.HasValue) return this;
        return new Weight(Value.Value / 2.20462, WeightUnit.Kg);
    }

    public Weight ToLbs()
    {
        if (Unit == WeightUnit.Lbs || !Value.HasValue) return this;
        return new Weight(Value.Value * 2.20462, WeightUnit.Lbs);
    }
    
    public override string ToString() => Value.HasValue && Unit.HasValue ? $"{Value.Value:N2} {Unit.Value}" : "";
}