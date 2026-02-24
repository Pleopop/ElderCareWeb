using MediatR;
using ElderCare.Application.Features.FraudDetection.Queries;
using ElderCare.Application.Services;
using ElderCare.Domain.Entities;

namespace ElderCare.Application.Features.FraudDetection.Handlers;

public class GetFraudAlertsHandler : IRequestHandler<GetFraudAlertsQuery, List<FraudAlert>>
{
    private readonly IFraudDetectionService _fraudService;
    
    public GetFraudAlertsHandler(IFraudDetectionService fraudService)
    {
        _fraudService = fraudService;
    }
    
    public async Task<List<FraudAlert>> Handle(GetFraudAlertsQuery request, CancellationToken cancellationToken)
    {
        return await _fraudService.GetAlertsAsync(request.UserId, request.Status);
    }
}

public class GetFraudScoreHistoryHandler : IRequestHandler<GetFraudScoreHistoryQuery, List<FraudScore>>
{
    private readonly IFraudDetectionService _fraudService;
    
    public GetFraudScoreHistoryHandler(IFraudDetectionService fraudService)
    {
        _fraudService = fraudService;
    }
    
    public async Task<List<FraudScore>> Handle(GetFraudScoreHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _fraudService.GetFraudScoreHistoryAsync(request.UserId);
    }
}

public class GetSuspiciousActivitiesHandler : IRequestHandler<GetSuspiciousActivitiesQuery, List<SuspiciousActivity>>
{
    private readonly IFraudDetectionService _fraudService;
    
    public GetSuspiciousActivitiesHandler(IFraudDetectionService fraudService)
    {
        _fraudService = fraudService;
    }
    
    public async Task<List<SuspiciousActivity>> Handle(GetSuspiciousActivitiesQuery request, CancellationToken cancellationToken)
    {
        return await _fraudService.GetSuspiciousActivitiesAsync(request.UserId, request.MinRiskScore);
    }
}
