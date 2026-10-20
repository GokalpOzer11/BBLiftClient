using BBLiftClient.Application.Enums;

namespace BBLiftClient.Application.Dtos;

public record CreatePaymentRequest
(
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method
);