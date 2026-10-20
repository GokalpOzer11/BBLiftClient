using BBLiftClient.Application.Enums;

namespace BBLiftClient.Application.Dtos;

public record MoneyResponse(decimal Amount, string Currency);

public record PaymentResponse
(
    Guid Id,
    Guid OrderId,
    MoneyResponse Amount,
    PaymentMethod Method,
    PaymentStatus Status,
    DateTime CreatedOn
);