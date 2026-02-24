using MediatR;
using ElderCare.Domain.Entities;

namespace ElderCare.Application.Features.FraudDetection.Commands;

/// <summary>
/// Calculate fraud score for a user
/// </summary>
public record CalculateFraudScoreCommand(Guid UserId) : IRequest<FraudScore>;

/// <summary>
/// Resolve a fraud alert
/// </summary>
public record ResolveAlertCommand(
    Guid AlertId,
    string Resolution,
    string InvestigatedBy
) : IRequest;

/// <summary>
/// Create a fraud alert manually
/// </summary>
public record CreateFraudAlertCommand(
    Guid UserId,
    string AlertType,
    int Severity,
    string Description
) : IRequest<FraudAlert>;
