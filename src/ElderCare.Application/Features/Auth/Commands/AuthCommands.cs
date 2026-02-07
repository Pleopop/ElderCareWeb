using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Auth.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Application.Features.Auth.Commands;

// Register Customer Command
public record RegisterCustomerCommand(RegisterCustomerRequest Request) : IRequest<Result<AuthResponse>>;

public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Request.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");
    }
}

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure("Registration failed", "Email already registered");
        }

        // Check phone if provided
        if (!string.IsNullOrEmpty(request.Request.PhoneNumber))
        {
            var existingPhone = await _unitOfWork.Users.GetByPhoneAsync(request.Request.PhoneNumber, cancellationToken);
            if (existingPhone != null)
            {
                return Result<AuthResponse>.Failure("Registration failed", "Phone number already registered");
            }
        }

        // Create user
        var user = new User
        {
            Email = request.Request.Email,
            PhoneNumber = request.Request.PhoneNumber,
            PasswordHash = _passwordHasher.HashPassword(request.Request.Password),
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        // Create customer profile
        var Customer = new Customer
        {
            UserId = user.Id,
            FullName = request.Request.FullName,
            Address = request.Request.Address
        };

        await _unitOfWork.Customers.AddAsync(Customer, cancellationToken);

        // Create wallet
        var wallet = new Wallet
        {
            UserId = user.Id
        };

        await _unitOfWork.Wallets.AddAsync(wallet, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified
            }
        };

        return Result<AuthResponse>.Success(response, "Customer registered successfully");
    }
}

// Register Caregiver Command
public record RegisterCaregiverCommand(RegisterCaregiverRequest Request) : IRequest<Result<AuthResponse>>;

public class RegisterCaregiverCommandValidator : AbstractValidator<RegisterCaregiverCommand>
{
    public RegisterCaregiverCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Request.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");
    }
}

public class RegisterCaregiverCommandHandler : IRequestHandler<RegisterCaregiverCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCaregiverCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCaregiverCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure("Registration failed", "Email already registered");
        }

        // Check phone if provided
        if (!string.IsNullOrEmpty(request.Request.PhoneNumber))
        {
            var existingPhone = await _unitOfWork.Users.GetByPhoneAsync(request.Request.PhoneNumber, cancellationToken);
            if (existingPhone != null)
            {
                return Result<AuthResponse>.Failure("Registration failed", "Phone number already registered");
            }
        }

        // Create user
        var user = new User
        {
            Email = request.Request.Email,
            PhoneNumber = request.Request.PhoneNumber,
            PasswordHash = _passwordHasher.HashPassword(request.Request.Password),
            Role = UserRole.Caregiver,
            Status = UserStatus.Active
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        // Create caregiver profile
        var Caregiver = new Caregiver
        {
            UserId = user.Id,
            FullName = request.Request.FullName,
            VerificationStatus = VerificationStatus.Pending
        };

        await _unitOfWork.Caregivers.AddAsync(Caregiver, cancellationToken);

        // Create wallet
        var wallet = new Wallet
        {
            UserId = user.Id
        };

        await _unitOfWork.Wallets.AddAsync(wallet, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified
            }
        };

        return Result<AuthResponse>.Success(response, "Caregiver registered successfully. Please complete eKYC verification.");
    }
}

// Login Command
public record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.EmailOrPhone)
            .NotEmpty().WithMessage("Email or phone number is required");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email or phone
        User? user = null;
        
        if (request.Request.EmailOrPhone.Contains("@"))
        {
            user = await _unitOfWork.Users.GetByEmailAsync(request.Request.EmailOrPhone, cancellationToken);
        }
        else
        {
            user = await _unitOfWork.Users.GetByPhoneAsync(request.Request.EmailOrPhone, cancellationToken);
        }

        if (user == null)
        {
            return Result<AuthResponse>.Failure("Login failed", "Invalid credentials");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Request.Password))
        {
            return Result<AuthResponse>.Failure("Login failed", "Invalid credentials");
        }

        // Check user status
        if (user.Status != UserStatus.Active)
        {
            return Result<AuthResponse>.Failure("Login failed", $"Account is {user.Status}");
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified
            }
        };

        return Result<AuthResponse>.Success(response, "Login successful");
    }
}
