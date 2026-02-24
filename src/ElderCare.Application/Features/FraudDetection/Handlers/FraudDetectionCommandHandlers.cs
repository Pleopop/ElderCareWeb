using MediatR;
using ElderCare.Application.Features.FraudDetection.Commands;
using ElderCare.Application.Services;
using ElderCare.Domain.Entities;
namespace ElderCare.Application.Features.FraudDetection.Handlers;

public class CalculateFraudScoreHandler : IRequestHandler<CalculateFraudScoreCommand, FraudScore>
{
    private readonly IFraudDetectionService _fraudService;
    
    public CalculateFraudScoreHandler(IFraudDetectionService fraudService)
    {
        _fraudService = fraudService;
    }
    
    public async Task<FraudScore> Handle(CalculateFraudScoreCommand request, CancellationToken cancellationToken)
    {
        return await _fraudService.CalculateFraudScoreAsync(request.UserId);
    }
}

public class ResolveAlertHandler : IRequestHandler<ResolveAlertCommand>
{
    private readonly IFraudDetectionService _fraudService;
    
    public ResolveAlertHandler(IFraudDetectionService fraudService)
    {
        _fraudService = fraudService;
    }
    
    public async Task Handle(ResolveAlertCommand request, CancellationToken cancellationToken)
    {
        await _fraudService.ResolveAlertAsync(
            request.AlertId,
            request.Resolution,
            request.InvestigatedBy
        );
    }
}

public class CreateFraudAlertHandler : IRequestHandler<CreateFraudAlertCommand, FraudAlert>
{
    private readonly IFraudDetectionService _fraudService;
    
    public CreateFraudAlertHandler(IFraudDetectionService fraudService)
    {
        _fraudService = fraudService;
    }
    
    public async Task<FraudAlert> Handle(CreateFraudAlertCommand request, CancellationToken cancellationToken)
    {
        return await _fraudService.CreateAlertAsync(
            request.UserId,
            request.AlertType,
            request.Severity,
            request.Description
        );
    }
}
