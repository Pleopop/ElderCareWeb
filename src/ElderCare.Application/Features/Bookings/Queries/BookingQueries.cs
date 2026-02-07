using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Bookings.DTOs;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Application.Features.Bookings.Queries;

// Get My Bookings (Customer)
public record GetMyBookingsQuery(BookingStatus? Status = null) : IRequest<Result<List<BookingDto>>>;

public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, Result<List<BookingDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyBookingsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<BookingDto>>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var Customer = await _unitOfWork.Customers.Query()
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId, cancellationToken);

        if (Customer == null)
            return Result<List<BookingDto>>.Failure("Not found", "Customer profile not found");

        var query = _unitOfWork.Bookings.Query()
            .Include(b => b.Caregiver)
            .Include(b => b.Beneficiary)
            .Where(b => b.CustomerId == Customer.Id);

        if (request.Status.HasValue)
            query = query.Where(b => b.Status == request.Status.Value);

        var bookings = await query.OrderByDescending(b => b.ScheduledStartTime).ToListAsync(cancellationToken);

        var dtos = bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            CustomerId = b.CustomerId,
            CustomerName = Customer.FullName,
            CaregiverId = b.CaregiverId,
            CaregiverName = b.Caregiver.FullName,
            BeneficiaryId = b.BeneficiaryId,
            BeneficiaryName = b.Beneficiary.FullName,
            ScheduledStartTime = b.ScheduledStartTime,
            ScheduledEndTime = b.ScheduledEndTime,
            ActualStartTime = b.ActualStartTime,
            ActualEndTime = b.ActualEndTime,
            ServiceLocation = b.ServiceLocation,
            Status = b.Status,
            TotalAmount = b.TotalAmount,
            SpecialRequirements = b.SpecialRequirements,
            AiMatchScore = b.AiMatchScore,
            CreatedAt = b.CreatedAt
        }).ToList();

        return Result<List<BookingDto>>.Success(dtos, "Bookings retrieved successfully");
    }
}

// Get Caregiver Bookings
public record GetCaregiverBookingsQuery(BookingStatus? Status = null) : IRequest<Result<List<BookingDto>>>;

public class GetCaregiverBookingsQueryHandler : IRequestHandler<GetCaregiverBookingsQuery, Result<List<BookingDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetCaregiverBookingsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<BookingDto>>> Handle(GetCaregiverBookingsQuery request, CancellationToken cancellationToken)
    {
        var Caregiver = await _unitOfWork.Caregivers.Query()
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId, cancellationToken);

        if (Caregiver == null)
            return Result<List<BookingDto>>.Failure("Not found", "Caregiver profile not found");

        var query = _unitOfWork.Bookings.Query()
            .Include(b => b.Customer)
            .Include(b => b.Beneficiary)
            .Where(b => b.CaregiverId == Caregiver.Id);

        if (request.Status.HasValue)
            query = query.Where(b => b.Status == request.Status.Value);

        var bookings = await query.OrderByDescending(b => b.ScheduledStartTime).ToListAsync(cancellationToken);

        var dtos = bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            CustomerId = b.CustomerId,
            CustomerName = b.Customer.FullName,
            CaregiverId = b.CaregiverId,
            CaregiverName = Caregiver.FullName,
            BeneficiaryId = b.BeneficiaryId,
            BeneficiaryName = b.Beneficiary.FullName,
            ScheduledStartTime = b.ScheduledStartTime,
            ScheduledEndTime = b.ScheduledEndTime,
            ActualStartTime = b.ActualStartTime,
            ActualEndTime = b.ActualEndTime,
            ServiceLocation = b.ServiceLocation,
            Status = b.Status,
            TotalAmount = b.TotalAmount,
            SpecialRequirements = b.SpecialRequirements,
            CreatedAt = b.CreatedAt
        }).ToList();

        return Result<List<BookingDto>>.Success(dtos, "Bookings retrieved successfully");
    }
}
