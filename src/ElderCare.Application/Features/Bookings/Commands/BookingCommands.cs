using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Bookings.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Application.Features.Bookings.Commands;

// Create Booking Command
public record CreateBookingCommand(CreateBookingRequest Request) : IRequest<Result<BookingDto>>;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.Request.BeneficiaryId).NotEmpty();
        RuleFor(x => x.Request.CaregiverId).NotEmpty();
        RuleFor(x => x.Request.ScheduledStartTime).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.Request.ScheduledEndTime).GreaterThan(x => x.Request.ScheduledStartTime);
        RuleFor(x => x.Request.ServiceLocation).NotEmpty().MaximumLength(500);
    }
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null)
            return Result<BookingDto>.Failure("Unauthorized", "User not authenticated");

        // Get customer profile
        var Customer = await _unitOfWork.Customers.Query()
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId.Value, cancellationToken);

        if (Customer == null)
            return Result<BookingDto>.Failure("Not found", "Customer profile not found");

        // Verify beneficiary belongs to customer
        var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(request.Request.BeneficiaryId, cancellationToken);
        if (beneficiary == null || beneficiary.CustomerId != Customer.Id)
            return Result<BookingDto>.Failure("Invalid", "Beneficiary not found or does not belong to you");

        // Verify caregiver exists and is approved
        var caregiver = await _unitOfWork.Caregivers.GetByIdAsync(request.Request.CaregiverId, cancellationToken);
        if (caregiver == null || caregiver.VerificationStatus != VerificationStatus.Approved)
            return Result<BookingDto>.Failure("Invalid", "Caregiver not available");

        // Calculate amount (hourly rate * hours)
        var hours = (request.Request.ScheduledEndTime - request.Request.ScheduledStartTime).TotalHours;
        var totalAmount = (decimal)hours * (caregiver.HourlyRate ?? 0);
        var commissionAmount = totalAmount * 0.1m; // 10% commission
        var escrowAmount = totalAmount;

        var booking = new Booking
        {
            CustomerId = Customer.Id,
            CaregiverId = request.Request.CaregiverId,
            BeneficiaryId = request.Request.BeneficiaryId,
            ScheduledStartTime = request.Request.ScheduledStartTime,
            ScheduledEndTime = request.Request.ScheduledEndTime,
            ServiceLocation = request.Request.ServiceLocation,
            Latitude = request.Request.Latitude,
            Longitude = request.Request.Longitude,
            SpecialRequirements = request.Request.SpecialRequirements,
            Status = BookingStatus.Pending,
            TotalAmount = totalAmount,
            EscrowAmount = escrowAmount,
            CommissionAmount = commissionAmount
        };

        await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new BookingDto
        {
            Id = booking.Id,
            CustomerId = Customer.Id,
            CustomerName = Customer.FullName,
            CaregiverId = caregiver.Id,
            CaregiverName = caregiver.FullName,
            BeneficiaryId = beneficiary.Id,
            BeneficiaryName = beneficiary.FullName,
            ScheduledStartTime = booking.ScheduledStartTime,
            ScheduledEndTime = booking.ScheduledEndTime,
            ServiceLocation = booking.ServiceLocation,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            SpecialRequirements = booking.SpecialRequirements,
            CreatedAt = booking.CreatedAt
        };

        return Result<BookingDto>.Success(dto, "Booking created successfully");
    }
}

// Accept Booking Command
public record AcceptBookingCommand(Guid BookingId) : IRequest<Result<BookingDto>>;

public class AcceptBookingCommandHandler : IRequestHandler<AcceptBookingCommand, Result<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AcceptBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BookingDto>> Handle(AcceptBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.Query()
            .Include(b => b.Customer)
            .Include(b => b.Caregiver)
            .Include(b => b.Beneficiary)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
            return Result<BookingDto>.Failure("Not found", "Booking not found");

        // Verify caregiver
        var Caregiver = await _unitOfWork.Caregivers.Query()
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId, cancellationToken);

        if (Caregiver == null || booking.CaregiverId != Caregiver.Id)
            return Result<BookingDto>.Failure("Unauthorized", "Not authorized to accept this booking");

        if (booking.Status != BookingStatus.Pending)
            return Result<BookingDto>.Failure("Invalid", "Booking cannot be accepted in current status");

        booking.Status = BookingStatus.Accepted;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new BookingDto
        {
            Id = booking.Id,
            CustomerId = booking.Customer.Id,
            CustomerName = booking.Customer.FullName,
            CaregiverId = booking.Caregiver.Id,
            CaregiverName = booking.Caregiver.FullName,
            BeneficiaryId = booking.Beneficiary.Id,
            BeneficiaryName = booking.Beneficiary.FullName,
            ScheduledStartTime = booking.ScheduledStartTime,
            ScheduledEndTime = booking.ScheduledEndTime,
            ServiceLocation = booking.ServiceLocation,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            CreatedAt = booking.CreatedAt
        };

        return Result<BookingDto>.Success(dto, "Booking accepted successfully");
    }
}

// Reject/Cancel Booking Command
public record CancelBookingCommand(Guid BookingId, string Reason) : IRequest<Result<BookingDto>>;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BookingDto>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.Query()
            .Include(b => b.Customer)
            .Include(b => b.Caregiver)
            .Include(b => b.Beneficiary)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
            return Result<BookingDto>.Failure("Not found", "Booking not found");

        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = request.Reason;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new BookingDto
        {
            Id = booking.Id,
            CustomerId = booking.Customer.Id,
            CustomerName = booking.Customer.FullName,
            CaregiverId = booking.Caregiver.Id,
            CaregiverName = booking.Caregiver.FullName,
            BeneficiaryId = booking.Beneficiary.Id,
            BeneficiaryName = booking.Beneficiary.FullName,
            ScheduledStartTime = booking.ScheduledStartTime,
            ScheduledEndTime = booking.ScheduledEndTime,
            ServiceLocation = booking.ServiceLocation,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            CreatedAt = booking.CreatedAt
        };

        return Result<BookingDto>.Success(dto, "Booking cancelled successfully");
    }
}

// Check-in Command
public record CheckInCommand(CheckInRequest Request) : IRequest<Result<BookingDto>>;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, Result<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckInCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BookingDto>> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.Query()
            .Include(b => b.Customer)
            .Include(b => b.Caregiver)
            .Include(b => b.Beneficiary)
            .FirstOrDefaultAsync(b => b.Id == request.Request.BookingId, cancellationToken);

        if (booking == null)
            return Result<BookingDto>.Failure("Not found", "Booking not found");

        if (booking.Status != BookingStatus.Accepted)
            return Result<BookingDto>.Failure("Invalid", "Booking must be accepted before check-in");

        booking.ActualStartTime = DateTime.UtcNow;
        booking.CheckInLatitude = request.Request.Latitude;
        booking.CheckInLongitude = request.Request.Longitude;
        booking.CheckInPhotoUrl = request.Request.PhotoUrl;
        booking.Status = BookingStatus.InProgress;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new BookingDto
        {
            Id = booking.Id,
            CustomerId = booking.Customer.Id,
            CustomerName = booking.Customer.FullName,
            CaregiverId = booking.Caregiver.Id,
            CaregiverName = booking.Caregiver.FullName,
            BeneficiaryId = booking.Beneficiary.Id,
            BeneficiaryName = booking.Beneficiary.FullName,
            ScheduledStartTime = booking.ScheduledStartTime,
            ScheduledEndTime = booking.ScheduledEndTime,
            ActualStartTime = booking.ActualStartTime,
            ServiceLocation = booking.ServiceLocation,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            CreatedAt = booking.CreatedAt
        };

        return Result<BookingDto>.Success(dto, "Check-in successful");
    }
}

// Check-out Command
public record CheckOutCommand(CheckOutRequest Request) : IRequest<Result<BookingDto>>;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, Result<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckOutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BookingDto>> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.Query()
            .Include(b => b.Customer)
            .Include(b => b.Caregiver)
            .Include(b => b.Beneficiary)
            .FirstOrDefaultAsync(b => b.Id == request.Request.BookingId, cancellationToken);

        if (booking == null)
            return Result<BookingDto>.Failure("Not found", "Booking not found");

        if (booking.Status != BookingStatus.InProgress)
            return Result<BookingDto>.Failure("Invalid", "Booking must be in progress before check-out");

        booking.ActualEndTime = DateTime.UtcNow;
        booking.CheckOutLatitude = request.Request.Latitude;
        booking.CheckOutLongitude = request.Request.Longitude;
        booking.CheckOutNotes = request.Request.Notes;
        booking.Status = BookingStatus.Completed;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new BookingDto
        {
            Id = booking.Id,
            CustomerId = booking.Customer.Id,
            CustomerName = booking.Customer.FullName,
            CaregiverId = booking.Caregiver.Id,
            CaregiverName = booking.Caregiver.FullName,
            BeneficiaryId = booking.Beneficiary.Id,
            BeneficiaryName = booking.Beneficiary.FullName,
            ScheduledStartTime = booking.ScheduledStartTime,
            ScheduledEndTime = booking.ScheduledEndTime,
            ActualStartTime = booking.ActualStartTime,
            ActualEndTime = booking.ActualEndTime,
            ServiceLocation = booking.ServiceLocation,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            CreatedAt = booking.CreatedAt
        };

        return Result<BookingDto>.Success(dto, "Check-out successful");
    }
}
