using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Reviews.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Application.Features.Reviews.Commands;

public record CreateReviewCommand(CreateReviewRequest Request) : IRequest<Result<ReviewDto>>;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.Request.BookingId).NotEmpty();
        RuleFor(x => x.Request.OverallRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.PunctualityRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.ProfessionalismRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.CommunicationRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.CareQualityRating).InclusiveBetween(1, 5);
    }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<ReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.Query()
            .Include(b => b.CaregiverProfile)
            .Include(b => b.Review)
            .FirstOrDefaultAsync(b => b.Id == request.Request.BookingId, cancellationToken);

        if (booking == null)
            return Result<ReviewDto>.Failure("Not found", "Booking not found");

        if (booking.Status != BookingStatus.Completed)
            return Result<ReviewDto>.Failure("Invalid", "Can only review completed bookings");

        if (booking.Review != null)
            return Result<ReviewDto>.Failure("Invalid", "Booking already reviewed");

        var review = new Review
        {
            BookingId = booking.Id,
            CaregiverId = booking.CaregiverProfileId,
            OverallRating = request.Request.OverallRating,
            PunctualityRating = request.Request.PunctualityRating,
            ProfessionalismRating = request.Request.ProfessionalismRating,
            CommunicationRating = request.Request.CommunicationRating,
            CareQualityRating = request.Request.CareQualityRating,
            Comment = request.Request.Comment
        };

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);

        // Update caregiver rating
        var allReviews = await _unitOfWork.Reviews.Query()
            .Where(r => r.CaregiverId == booking.CaregiverProfileId)
            .ToListAsync(cancellationToken);

        allReviews.Add(review);

        var caregiver = booking.CaregiverProfile;
        caregiver.AverageRating = allReviews.Average(r => r.OverallRating);
        caregiver.TotalReviews = allReviews.Count;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new ReviewDto
        {
            Id = review.Id,
            BookingId = review.BookingId,
            CaregiverId = review.CaregiverId,
            CaregiverName = caregiver.FullName,
            OverallRating = review.OverallRating,
            PunctualityRating = review.PunctualityRating,
            ProfessionalismRating = review.ProfessionalismRating,
            CommunicationRating = review.CommunicationRating,
            CareQualityRating = review.CareQualityRating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };

        return Result<ReviewDto>.Success(dto, "Review created successfully");
    }
}
