using BBLiftClient.Application.Enums;

namespace BBLiftClient.Application.Dtos;

public record UpdatePaymentRequest
(
    Guid PaymentId,
    PaymentStatus Status
);