using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Tracking.DTOs;
using MediatR;

namespace ElderCare.Application.Features.Tracking.Queries;

// ============================================
// 1. Get Current Location Query
// ============================================
public record GetCurrentLocationQuery(Guid BookingId) : IRequest<Result<LocationDto>>;

public class GetCurrentLocationQueryHandler : IRequestHandler<GetCurrentLocationQuery, Result<LocationDto>>
{
    private readonly ILocationService _locationService;

    public GetCurrentLocationQueryHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<Result<LocationDto>> Handle(GetCurrentLocationQuery request, CancellationToken cancellationToken)
    {
        var currentLocation = await _locationService.GetCurrentLocationAsync(request.BookingId);
        
        if (currentLocation == null)
            return Result<LocationDto>.Failure("No location data available for this booking");

        var locationDto = new LocationDto
        {
            Id = currentLocation.Id,
            BookingId = currentLocation.BookingId,
            Latitude = currentLocation.Latitude,
            Longitude = currentLocation.Longitude,
            Accuracy = currentLocation.Accuracy,
            Timestamp = currentLocation.Timestamp
        };

        return Result<LocationDto>.Success(locationDto);
    }
}

// ============================================
// 2. Get Location History Query
// ============================================
public record GetLocationHistoryQuery(
    Guid BookingId,
    DateTime? From = null,
    DateTime? To = null
) : IRequest<Result<List<LocationDto>>>;

public class GetLocationHistoryQueryHandler : IRequestHandler<GetLocationHistoryQuery, Result<List<LocationDto>>>
{
    private readonly ILocationService _locationService;

    public GetLocationHistoryQueryHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<Result<List<LocationDto>>> Handle(GetLocationHistoryQuery request, CancellationToken cancellationToken)
    {
        var locationHistory = await _locationService.GetLocationHistoryAsync(
            request.BookingId,
            request.From,
            request.To
        );

        var locationDtos = locationHistory.Select(l => new LocationDto
        {
            Id = l.Id,
            BookingId = l.BookingId,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            Accuracy = l.Accuracy,
            Timestamp = l.Timestamp
        }).ToList();

        return Result<List<LocationDto>>.Success(locationDtos);
    }
}
