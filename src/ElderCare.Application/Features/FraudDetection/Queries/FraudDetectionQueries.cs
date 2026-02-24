using MediatR;
using ElderCare.Domain.Entities;

namespace ElderCare.Application.Features.FraudDetection.Queries;

/// <summary>
/// Get fraud alerts with optional filtering
/// </summary>
public record GetFraudAlertsQuery(
    Guid? UserId = null,
    string? Status = null
) : IRequest<List<FraudAlert>>;

/// <summary>
/// Get fraud score history for a user
/// </summary>
public record GetFraudScoreHistoryQuery(Guid UserId) : IRequest<List<FraudScore>>;

/// <summary>
/// Get suspicious activities for a user
/// </summary>
public record GetSuspiciousActivitiesQuery(
    Guid? UserId = null,
    int? MinRiskScore = null
) : IRequest<List<SuspiciousActivity>>;
