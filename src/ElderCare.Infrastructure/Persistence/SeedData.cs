using ElderCare.Application.Common.Interfaces;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Infrastructure.Persistence;

/// <summary>
/// Seed sample data for testing all features.
/// Idempotent: only seeds when Users table is empty.
/// Uses fixed GUIDs for consistent FK references.
/// </summary>
public static class SeedData
{
    // ==================== FIXED IDs ====================
    // Users
    private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid Customer1UserId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid Customer2UserId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    private static readonly Guid Caregiver1UserId = Guid.Parse("00000000-0000-0000-0000-000000000004");
    private static readonly Guid Caregiver2UserId = Guid.Parse("00000000-0000-0000-0000-000000000005");
    private static readonly Guid Caregiver3UserId = Guid.Parse("00000000-0000-0000-0000-000000000006");

    // Customers
    private static readonly Guid Customer1Id = Guid.Parse("00000000-0000-0000-0001-000000000001");
    private static readonly Guid Customer2Id = Guid.Parse("00000000-0000-0000-0001-000000000002");

    // Caregivers
    private static readonly Guid Caregiver1Id = Guid.Parse("00000000-0000-0000-0002-000000000001");
    private static readonly Guid Caregiver2Id = Guid.Parse("00000000-0000-0000-0002-000000000002");
    private static readonly Guid Caregiver3Id = Guid.Parse("00000000-0000-0000-0002-000000000003");

    // Beneficiaries
    private static readonly Guid Beneficiary1Id = Guid.Parse("00000000-0000-0000-0003-000000000001");
    private static readonly Guid Beneficiary2Id = Guid.Parse("00000000-0000-0000-0003-000000000002");
    private static readonly Guid Beneficiary3Id = Guid.Parse("00000000-0000-0000-0003-000000000003");

    // Bookings
    private static readonly Guid Booking1Id = Guid.Parse("00000000-0000-0000-0004-000000000001");
    private static readonly Guid Booking2Id = Guid.Parse("00000000-0000-0000-0004-000000000002");
    private static readonly Guid Booking3Id = Guid.Parse("00000000-0000-0000-0004-000000000003");
    private static readonly Guid Booking4Id = Guid.Parse("00000000-0000-0000-0004-000000000004");

    // Wallets
    private static readonly Guid Wallet1Id = Guid.Parse("00000000-0000-0000-0005-000000000001");
    private static readonly Guid Wallet2Id = Guid.Parse("00000000-0000-0000-0005-000000000002");
    private static readonly Guid Wallet3Id = Guid.Parse("00000000-0000-0000-0005-000000000003");
    private static readonly Guid Wallet4Id = Guid.Parse("00000000-0000-0000-0005-000000000004");
    private static readonly Guid Wallet5Id = Guid.Parse("00000000-0000-0000-0005-000000000005");
    private static readonly Guid Wallet6Id = Guid.Parse("00000000-0000-0000-0005-000000000006");

    // Conversations
    private static readonly Guid Conversation1Id = Guid.Parse("00000000-0000-0000-0006-000000000001");
    private static readonly Guid Conversation2Id = Guid.Parse("00000000-0000-0000-0006-000000000002");

    public static async Task SeedAsync(ElderCareDbContext context, IPasswordHasher passwordHasher)
    {
        // Only seed if DB is empty
        if (await context.Users.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var password = passwordHasher.HashPassword("Test@123");
        var adminPassword = passwordHasher.HashPassword("Admin@123");

        // ==================== 1. USERS ====================
        var users = new List<User>
        {
            new()
            {
                Id = AdminUserId, Email = "admin@eldercare.vn", PhoneNumber = "0900000001",
                PasswordHash = adminPassword, Role = UserRole.Admin, Status = UserStatus.Active,
                IsEmailVerified = true, IsPhoneVerified = true,
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Customer1UserId, Email = "customer1@gmail.com", PhoneNumber = "0912345001",
                PasswordHash = password, Role = UserRole.Customer, Status = UserStatus.Active,
                IsEmailVerified = true, IsPhoneVerified = true,
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Customer2UserId, Email = "customer2@gmail.com", PhoneNumber = "0912345002",
                PasswordHash = password, Role = UserRole.Customer, Status = UserStatus.Active,
                IsEmailVerified = true, IsPhoneVerified = false,
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Caregiver1UserId, Email = "caregiver1@gmail.com", PhoneNumber = "0912345003",
                PasswordHash = password, Role = UserRole.Caregiver, Status = UserStatus.Active,
                IsEmailVerified = true, IsPhoneVerified = true,
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Caregiver2UserId, Email = "caregiver2@gmail.com", PhoneNumber = "0912345004",
                PasswordHash = password, Role = UserRole.Caregiver, Status = UserStatus.Active,
                IsEmailVerified = true, IsPhoneVerified = true,
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Caregiver3UserId, Email = "caregiver3@gmail.com", PhoneNumber = "0912345005",
                PasswordHash = password, Role = UserRole.Caregiver, Status = UserStatus.Active,
                IsEmailVerified = true, IsPhoneVerified = false,
                CreatedAt = now, UpdatedAt = now
            }
        };
        await context.Users.AddRangeAsync(users);

        // ==================== 2. CUSTOMERS ====================
        var customers = new List<Customer>
        {
            new()
            {
                Id = Customer1Id, UserId = Customer1UserId,
                FullName = "Nguyễn Văn An", Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                Latitude = 10.7769, Longitude = 106.7009,
                EmergencyContactName = "Nguyễn Thị Lan", EmergencyContactPhone = "0901234567",
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Customer2Id, UserId = Customer2UserId,
                FullName = "Trần Thị Bình", Address = "456 Lê Lợi, Quận 3, TP.HCM",
                Latitude = 10.7756, Longitude = 106.6910,
                EmergencyContactName = "Trần Văn Minh", EmergencyContactPhone = "0909876543",
                CreatedAt = now, UpdatedAt = now
            }
        };
        await context.Customers.AddRangeAsync(customers);

        // ==================== 3. CAREGIVERS ====================
        var caregivers = new List<Caregiver>
        {
            new()
            {
                Id = Caregiver1Id, UserId = Caregiver1UserId,
                FullName = "Lê Thị Hương", IdentityNumber = "079200001234",
                Bio = "5 năm kinh nghiệm chăm sóc người cao tuổi. Chuyên về phục hồi chức năng và chăm sóc bệnh nhân Alzheimer.",
                ExperienceYears = 5, HourlyRate = 150000m, PersonalityType = "ENFJ",
                AverageRating = 4.8, TotalReviews = 12,
                Address = "789 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM",
                Latitude = 10.8012, Longitude = 106.7115, ServiceRadiusKm = 15,
                VerificationStatus = VerificationStatus.Approved,
                ApprovedAt = now.AddDays(-30), ApprovedBy = "admin@eldercare.vn",
                CreatedAt = now.AddDays(-60), UpdatedAt = now
            },
            new()
            {
                Id = Caregiver2Id, UserId = Caregiver2UserId,
                FullName = "Phạm Văn Đức", IdentityNumber = "079200005678",
                Bio = "Điều dưỡng viên với 3 năm kinh nghiệm tại bệnh viện. Kỹ năng theo dõi sức khỏe và hỗ trợ vận động.",
                ExperienceYears = 3, HourlyRate = 120000m, PersonalityType = "ISFJ",
                AverageRating = 4.5, TotalReviews = 8,
                Address = "321 Cách Mạng Tháng 8, Quận 10, TP.HCM",
                Latitude = 10.7725, Longitude = 106.6689, ServiceRadiusKm = 10,
                VerificationStatus = VerificationStatus.Approved,
                ApprovedAt = now.AddDays(-20), ApprovedBy = "admin@eldercare.vn",
                CreatedAt = now.AddDays(-45), UpdatedAt = now
            },
            new()
            {
                Id = Caregiver3Id, UserId = Caregiver3UserId,
                FullName = "Ngô Thị Lan", IdentityNumber = "079200009012",
                Bio = "Sinh viên Y khoa năm cuối, có kinh nghiệm chăm sóc người cao tuổi tại viện dưỡng lão.",
                ExperienceYears = 1, HourlyRate = 100000m, PersonalityType = "INFP",
                AverageRating = 0, TotalReviews = 0,
                Address = "567 Võ Văn Tần, Quận 3, TP.HCM",
                Latitude = 10.7780, Longitude = 106.6870, ServiceRadiusKm = 8,
                VerificationStatus = VerificationStatus.Pending, // Chưa được duyệt
                CreatedAt = now.AddDays(-5), UpdatedAt = now
            }
        };
        await context.Caregivers.AddRangeAsync(caregivers);

        // ==================== 4. CAREGIVER SKILLS ====================
        var skills = new List<CaregiverSkill>
        {
            // Caregiver 1 - Hương
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver1Id, SkillName = "Chăm sóc Alzheimer", Description = "Chuyên chăm sóc bệnh nhân Alzheimer và sa sút trí tuệ", ProficiencyLevel = 5, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver1Id, SkillName = "Phục hồi chức năng", Description = "Hỗ trợ vật lý trị liệu và phục hồi sau phẫu thuật", ProficiencyLevel = 4, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver1Id, SkillName = "Sơ cứu", Description = "Kỹ năng sơ cứu cơ bản và nâng cao", ProficiencyLevel = 5, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver1Id, SkillName = "Nấu ăn dinh dưỡng", Description = "Nấu ăn theo chế độ dinh dưỡng cho người già", ProficiencyLevel = 4, CreatedAt = now, UpdatedAt = now },
            // Caregiver 2 - Đức
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver2Id, SkillName = "Theo dõi sức khỏe", Description = "Đo huyết áp, đường huyết, nhiệt độ", ProficiencyLevel = 5, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver2Id, SkillName = "Hỗ trợ vận động", Description = "Hỗ trợ đi lại, tập thể dục nhẹ nhàng", ProficiencyLevel = 4, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver2Id, SkillName = "Sơ cứu", Description = "Kỹ năng sơ cứu cơ bản", ProficiencyLevel = 3, CreatedAt = now, UpdatedAt = now },
            // Caregiver 3 - Lan
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver3Id, SkillName = "Chăm sóc cơ bản", Description = "Hỗ trợ sinh hoạt hàng ngày", ProficiencyLevel = 3, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), CaregiverId = Caregiver3Id, SkillName = "Trò chuyện tâm lý", Description = "Lắng nghe và trò chuyện với người cao tuổi", ProficiencyLevel = 4, CreatedAt = now, UpdatedAt = now },
        };
        await context.CaregiverSkills.AddRangeAsync(skills);

        // ==================== 5. CAREGIVER AVAILABILITY ====================
        var availabilities = new List<CaregiverAvailability>();
        // Caregiver 1: Mon-Fri 7AM-5PM
        for (var d = DayOfWeek.Monday; d <= DayOfWeek.Friday; d++)
        {
            availabilities.Add(new() { Id = Guid.NewGuid(), CaregiverId = Caregiver1Id, DayOfWeek = d, StartTime = new TimeSpan(7, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsAvailable = true, CreatedAt = now, UpdatedAt = now });
        }
        // Caregiver 2: Mon-Sat 8AM-6PM
        for (var d = DayOfWeek.Monday; d <= DayOfWeek.Saturday; d++)
        {
            availabilities.Add(new() { Id = Guid.NewGuid(), CaregiverId = Caregiver2Id, DayOfWeek = d, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0), IsAvailable = true, CreatedAt = now, UpdatedAt = now });
        }
        // Caregiver 3: Sat-Sun 8AM-4PM (part-time student)
        availabilities.Add(new() { Id = Guid.NewGuid(), CaregiverId = Caregiver3Id, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(16, 0, 0), IsAvailable = true, CreatedAt = now, UpdatedAt = now });
        availabilities.Add(new() { Id = Guid.NewGuid(), CaregiverId = Caregiver3Id, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(16, 0, 0), IsAvailable = true, CreatedAt = now, UpdatedAt = now });
        await context.CaregiverAvailabilities.AddRangeAsync(availabilities);

        // ==================== 6. PERSONALITY ASSESSMENTS ====================
        var assessments = new List<PersonalityAssessment>
        {
            new()
            {
                Id = Guid.NewGuid(), CaregiverId = Caregiver1Id, PersonalityType = "ENFJ",
                ExtroversionScore = 85, PatienceScore = 90, EmpathyScore = 95,
                CommunicationScore = 88, FlexibilityScore = 80,
                CompletedAt = now.AddDays(-30), CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(), CaregiverId = Caregiver2Id, PersonalityType = "ISFJ",
                ExtroversionScore = 45, PatienceScore = 92, EmpathyScore = 85,
                CommunicationScore = 70, FlexibilityScore = 75,
                CompletedAt = now.AddDays(-20), CreatedAt = now, UpdatedAt = now
            }
        };
        await context.PersonalityAssessments.AddRangeAsync(assessments);

        // ==================== 7. BENEFICIARIES ====================
        var beneficiaries = new List<Beneficiary>
        {
            new()
            {
                Id = Beneficiary1Id, CustomerId = Customer1Id,
                FullName = "Nguyễn Văn Phúc", DateOfBirth = new DateTime(1948, 3, 15), Gender = Gender.Male,
                Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                MedicalConditions = "Tiểu đường type 2, Tăng huyết áp",
                Medications = "Metformin 500mg (2 lần/ngày), Amlodipine 5mg (1 lần/ngày)",
                Allergies = "Penicillin",
                MobilityLevel = MobilityLevel.SlightlyLimited, CognitiveStatus = CognitiveStatus.Normal,
                SpecialNeeds = "Cần theo dõi đường huyết mỗi ngày",
                PersonalityTraits = "Hiền lành, thích trò chuyện, hài hước",
                Hobbies = "Cờ tướng, đọc báo, trồng cây, xem thời sự",
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Beneficiary2Id, CustomerId = Customer1Id,
                FullName = "Trần Thị Mai", DateOfBirth = new DateTime(1944, 7, 22), Gender = Gender.Female,
                Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                MedicalConditions = "Alzheimer giai đoạn đầu, Loãng xương",
                Medications = "Donepezil 5mg (1 lần/ngày), Calcium + Vitamin D",
                Allergies = null,
                MobilityLevel = MobilityLevel.ModeratelyLimited, CognitiveStatus = CognitiveStatus.MildImpairment,
                SpecialNeeds = "Cần giám sát thường xuyên, hay quên uống thuốc",
                PersonalityTraits = "Nhẹ nhàng, hay lo lắng, thích sạch sẽ",
                Hobbies = "Nghe nhạc xưa, xem phim, đan len, nấu ăn",
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Beneficiary3Id, CustomerId = Customer2Id,
                FullName = "Nguyễn Bá Tùng", DateOfBirth = new DateTime(1951, 11, 8), Gender = Gender.Male,
                Address = "456 Lê Lợi, Quận 3, TP.HCM",
                MedicalConditions = "Parkinson giai đoạn 2, Thoái hóa khớp gối",
                Medications = "Levodopa 250mg (3 lần/ngày), Glucosamine",
                Allergies = "Hải sản",
                MobilityLevel = MobilityLevel.ModeratelyLimited, CognitiveStatus = CognitiveStatus.Normal,
                SpecialNeeds = "Cần hỗ trợ đi lại, tay hay run",
                PersonalityTraits = "Kiên nhẫn, ít nói, thích yên tĩnh",
                Hobbies = "Nghe radio, chơi cờ vua, viết thư pháp",
                CreatedAt = now, UpdatedAt = now
            }
        };
        await context.Beneficiaries.AddRangeAsync(beneficiaries);

        // ==================== 8. BENEFICIARY PREFERENCES ====================
        var preferences = new List<BeneficiaryPreference>
        {
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary1Id,
                PreferredGender = Gender.Female, PreferredAgeRange = "25-45",
                PreferredPersonalityTraits = "Kiên nhẫn, vui vẻ, hoạt bát",
                AvoidPersonalityTraits = "Nóng tính, ít nói",
                SpecialRequirements = "Biết nấu ăn cho người tiểu đường",
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary2Id,
                PreferredGender = Gender.Female, PreferredAgeRange = "30-50",
                PreferredPersonalityTraits = "Nhẹ nhàng, kiên nhẫn, cẩn thận",
                AvoidPersonalityTraits = null,
                SpecialRequirements = "Có kinh nghiệm chăm sóc bệnh nhân Alzheimer",
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary3Id,
                PreferredGender = null, PreferredAgeRange = "20-40",
                PreferredPersonalityTraits = "Kiên nhẫn, mạnh mẽ",
                AvoidPersonalityTraits = null,
                SpecialRequirements = "Có khả năng hỗ trợ vật lý trị liệu",
                CreatedAt = now, UpdatedAt = now
            }
        };
        await context.BeneficiaryPreferences.AddRangeAsync(preferences);

        // ==================== 9. WALLETS ====================
        var wallets = new List<Wallet>
        {
            new() { Id = Wallet1Id, UserId = AdminUserId, Balance = 0m, EscrowBalance = 0m, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new() { Id = Wallet2Id, UserId = Customer1UserId, Balance = 10000000m, EscrowBalance = 600000m, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new() { Id = Wallet3Id, UserId = Customer2UserId, Balance = 5000000m, EscrowBalance = 0m, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new() { Id = Wallet4Id, UserId = Caregiver1UserId, Balance = 3200000m, EscrowBalance = 0m, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new() { Id = Wallet5Id, UserId = Caregiver2UserId, Balance = 1500000m, EscrowBalance = 0m, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new() { Id = Wallet6Id, UserId = Caregiver3UserId, Balance = 0m, EscrowBalance = 0m, IsActive = true, CreatedAt = now, UpdatedAt = now },
        };
        await context.Wallets.AddRangeAsync(wallets);

        // ==================== 10. BOOKINGS ====================
        var bookings = new List<Booking>
        {
            // Booking 1: Completed
            new()
            {
                Id = Booking1Id, CustomerId = Customer1Id, CaregiverId = Caregiver1Id, BeneficiaryId = Beneficiary1Id,
                ScheduledStartTime = now.AddDays(-7).Date.AddHours(8), ScheduledEndTime = now.AddDays(-7).Date.AddHours(12),
                ActualStartTime = now.AddDays(-7).Date.AddHours(7).AddMinutes(55), ActualEndTime = now.AddDays(-7).Date.AddHours(12).AddMinutes(5),
                Status = BookingStatus.Completed, TotalAmount = 600000m, CommissionAmount = 60000m,
                ServiceLocation = "123 Nguyễn Huệ, Quận 1, TP.HCM", Latitude = 10.7769, Longitude = 106.7009,
                GeofenceRadiusMeters = 100, AiMatchScore = 92.5,
                CheckInLatitude = 10.7770, CheckInLongitude = 106.7010,
                CheckOutLatitude = 10.7769, CheckOutLongitude = 106.7009, CheckOutNotes = "Ông Phúc khỏe, đã ăn trưa đầy đủ.",
                Notes = "Chăm sóc buổi sáng, theo dõi đường huyết",
                EscrowAmount = 600000m, EscrowHeldAt = now.AddDays(-8), EscrowReleasedAt = now.AddDays(-7),
                CreatedAt = now.AddDays(-8), UpdatedAt = now.AddDays(-7)
            },
            // Booking 2: InProgress
            new()
            {
                Id = Booking2Id, CustomerId = Customer1Id, CaregiverId = Caregiver1Id, BeneficiaryId = Beneficiary2Id,
                ScheduledStartTime = now.Date.AddHours(8), ScheduledEndTime = now.Date.AddHours(16),
                ActualStartTime = now.Date.AddHours(8).AddMinutes(2),
                Status = BookingStatus.InProgress, TotalAmount = 1200000m, CommissionAmount = 120000m,
                ServiceLocation = "123 Nguyễn Huệ, Quận 1, TP.HCM", Latitude = 10.7769, Longitude = 106.7009,
                GeofenceRadiusMeters = 100, AiMatchScore = 88.0,
                CheckInLatitude = 10.7770, CheckInLongitude = 106.7008,
                Notes = "Chăm sóc cả ngày, giám sát uống thuốc Alzheimer",
                EscrowAmount = 1200000m, EscrowHeldAt = now.AddDays(-1),
                CreatedAt = now.AddDays(-1), UpdatedAt = now
            },
            // Booking 3: Pending (chờ caregiver accept)
            new()
            {
                Id = Booking3Id, CustomerId = Customer2Id, CaregiverId = Caregiver2Id, BeneficiaryId = Beneficiary3Id,
                ScheduledStartTime = now.AddDays(2).Date.AddHours(9), ScheduledEndTime = now.AddDays(2).Date.AddHours(15),
                Status = BookingStatus.Pending, TotalAmount = 720000m, CommissionAmount = 72000m,
                ServiceLocation = "456 Lê Lợi, Quận 3, TP.HCM", Latitude = 10.7756, Longitude = 106.6910,
                GeofenceRadiusMeters = 100, AiMatchScore = 75.5,
                Notes = "Hỗ trợ vận động và vật lý trị liệu cho ông Tùng",
                CreatedAt = now, UpdatedAt = now
            },
            // Booking 4: Cancelled
            new()
            {
                Id = Booking4Id, CustomerId = Customer2Id, CaregiverId = Caregiver1Id, BeneficiaryId = Beneficiary3Id,
                ScheduledStartTime = now.AddDays(-3).Date.AddHours(8), ScheduledEndTime = now.AddDays(-3).Date.AddHours(12),
                Status = BookingStatus.Cancelled, TotalAmount = 600000m, CommissionAmount = 60000m,
                CancellationReason = "Gia đình có việc bận, xin hủy lịch",
                ServiceLocation = "456 Lê Lợi, Quận 3, TP.HCM", Latitude = 10.7756, Longitude = 106.6910,
                GeofenceRadiusMeters = 100,
                CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-3)
            }
        };
        await context.Bookings.AddRangeAsync(bookings);

        // ==================== 11. TRANSACTIONS ====================
        var transactions = new List<Transaction>
        {
            // Customer 1 deposit
            new()
            {
                Id = Guid.NewGuid(), WalletId = Wallet2Id, Type = TransactionType.Deposit,
                Amount = 15000000m, Description = "Nạp tiền vào ví", Status = TransactionStatus.Completed,
                CreatedAt = now.AddDays(-10), UpdatedAt = now.AddDays(-10)
            },
            // Escrow hold for Booking 1
            new()
            {
                Id = Guid.NewGuid(), WalletId = Wallet2Id, Type = TransactionType.EscrowHold,
                Amount = 600000m, Description = "Giữ tiền cho booking #1", Status = TransactionStatus.Completed,
                RelatedBookingId = Booking1Id,
                CreatedAt = now.AddDays(-8), UpdatedAt = now.AddDays(-8)
            },
            // Escrow release for Booking 1 → Caregiver 1
            new()
            {
                Id = Guid.NewGuid(), WalletId = Wallet4Id, Type = TransactionType.EscrowRelease,
                Amount = 540000m, Description = "Thanh toán booking #1 (sau trừ commission 10%)", Status = TransactionStatus.Completed,
                RelatedBookingId = Booking1Id,
                CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-7)
            },
            // Commission for Booking 1
            new()
            {
                Id = Guid.NewGuid(), WalletId = Wallet1Id, Type = TransactionType.Commission,
                Amount = 60000m, Description = "Hoa hồng booking #1 (10%)", Status = TransactionStatus.Completed,
                RelatedBookingId = Booking1Id,
                CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-7)
            },
            // Escrow hold for Booking 2
            new()
            {
                Id = Guid.NewGuid(), WalletId = Wallet2Id, Type = TransactionType.EscrowHold,
                Amount = 1200000m, Description = "Giữ tiền cho booking #2", Status = TransactionStatus.Completed,
                RelatedBookingId = Booking2Id,
                CreatedAt = now.AddDays(-1), UpdatedAt = now.AddDays(-1)
            },
            // Customer 2 deposit
            new()
            {
                Id = Guid.NewGuid(), WalletId = Wallet3Id, Type = TransactionType.Deposit,
                Amount = 5000000m, Description = "Nạp tiền vào ví", Status = TransactionStatus.Completed,
                CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-5)
            },
            // Refund for cancelled Booking 4
            new()
            {
                Id = Guid.NewGuid(), WalletId = Wallet3Id, Type = TransactionType.Refund,
                Amount = 600000m, Description = "Hoàn tiền booking #4 do hủy", Status = TransactionStatus.Completed,
                RelatedBookingId = Booking4Id,
                CreatedAt = now.AddDays(-3), UpdatedAt = now.AddDays(-3)
            }
        };
        await context.Transactions.AddRangeAsync(transactions);

        // ==================== 12. REVIEWS ====================
        var reviews = new List<Review>
        {
            new()
            {
                Id = Guid.NewGuid(), BookingId = Booking1Id, CaregiverId = Caregiver1Id,
                OverallRating = 5, PunctualityRating = 5, ProfessionalismRating = 5,
                CommunicationRating = 4, CareQualityRating = 5,
                Comment = "Cô Hương rất tận tâm, chăm sóc bố tôi rất chu đáo. Đường huyết bố ổn định suốt ngày. Rất hài lòng!",
                CreatedAt = now.AddDays(-6), UpdatedAt = now.AddDays(-6)
            }
        };
        await context.Reviews.AddRangeAsync(reviews);

        // ==================== 13. LOCATION LOGS (Booking 2 - InProgress) ====================
        var locationLogs = new List<LocationLog>();
        var logTime = now.Date.AddHours(8);
        for (int i = 0; i < 10; i++)
        {
            locationLogs.Add(new()
            {
                Id = Guid.NewGuid(), BookingId = Booking2Id,
                Latitude = 10.7769 + (i * 0.00001), Longitude = 106.7009 + (i * 0.00001),
                Accuracy = 5.0 + (i * 0.5), Timestamp = logTime.AddMinutes(i * 30),
                CreatedAt = logTime.AddMinutes(i * 30), UpdatedAt = logTime.AddMinutes(i * 30)
            });
        }
        await context.LocationLogs.AddRangeAsync(locationLogs);

        // ==================== 14. NOTIFICATIONS ====================
        var notifications = new List<Notification>
        {
            new()
            {
                Id = Guid.NewGuid(), UserId = Customer1UserId,
                Title = "Booking hoàn thành", Message = "Booking chăm sóc ông Phúc đã hoàn thành. Hãy đánh giá dịch vụ!",
                Type = NotificationType.Success, Category = NotificationCategory.Booking, Priority = NotificationPriority.Medium,
                IsRead = true, ReadAt = now.AddDays(-6), ActionUrl = "/bookings",
                RelatedEntityId = Booking1Id, RelatedEntityType = "Booking",
                CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-6)
            },
            new()
            {
                Id = Guid.NewGuid(), UserId = Customer1UserId,
                Title = "Booking đang diễn ra", Message = "Caregiver Lê Thị Hương đã check-in chăm sóc bà Mai.",
                Type = NotificationType.Info, Category = NotificationCategory.Booking, Priority = NotificationPriority.High,
                IsRead = false, ActionUrl = "/bookings",
                RelatedEntityId = Booking2Id, RelatedEntityType = "Booking",
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(), UserId = Caregiver1UserId,
                Title = "Thanh toán nhận được", Message = "Bạn đã nhận được 540,000 VND từ booking #1.",
                Type = NotificationType.Success, Category = NotificationCategory.Payment, Priority = NotificationPriority.Medium,
                IsRead = true, ReadAt = now.AddDays(-6),
                RelatedEntityId = Booking1Id, RelatedEntityType = "Transaction",
                CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-6)
            },
            new()
            {
                Id = Guid.NewGuid(), UserId = Caregiver2UserId,
                Title = "Booking mới", Message = "Bạn có booking mới từ khách hàng Trần Thị Bình. Vui lòng xác nhận.",
                Type = NotificationType.Info, Category = NotificationCategory.Booking, Priority = NotificationPriority.High,
                IsRead = false, ActionUrl = "/bookings",
                RelatedEntityId = Booking3Id, RelatedEntityType = "Booking",
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(), UserId = AdminUserId,
                Title = "Caregiver mới chờ duyệt", Message = "Ngô Thị Lan vừa đăng ký làm caregiver. Vui lòng xem xét hồ sơ.",
                Type = NotificationType.Warning, Category = NotificationCategory.Account, Priority = NotificationPriority.High,
                IsRead = false, ActionUrl = "/admin",
                RelatedEntityId = Caregiver3Id, RelatedEntityType = "Caregiver",
                CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-5)
            }
        };
        await context.Notifications.AddRangeAsync(notifications);

        // ==================== 15. CONVERSATIONS & MESSAGES ====================
        var conversations = new List<Conversation>
        {
            new()
            {
                Id = Conversation1Id, Type = ConversationType.OneOnOne, BookingId = Booking1Id,
                Title = null, IsArchived = false,
                LastMessageAt = now.AddDays(-6).AddHours(10),
                CreatedAt = now.AddDays(-8), UpdatedAt = now.AddDays(-6)
            },
            new()
            {
                Id = Conversation2Id, Type = ConversationType.OneOnOne, BookingId = Booking2Id,
                Title = null, IsArchived = false,
                LastMessageAt = now.AddHours(-1),
                CreatedAt = now.AddDays(-1), UpdatedAt = now
            }
        };
        await context.Conversations.AddRangeAsync(conversations);

        var participants = new List<ConversationParticipant>
        {
            // Conversation 1: Customer1 - Caregiver1
            new() { Id = Guid.NewGuid(), ConversationId = Conversation1Id, UserId = Customer1UserId, JoinedAt = now.AddDays(-8), LastReadAt = now.AddDays(-6), UnreadCount = 0, IsMuted = false, CreatedAt = now.AddDays(-8), UpdatedAt = now.AddDays(-6) },
            new() { Id = Guid.NewGuid(), ConversationId = Conversation1Id, UserId = Caregiver1UserId, JoinedAt = now.AddDays(-8), LastReadAt = now.AddDays(-6), UnreadCount = 0, IsMuted = false, CreatedAt = now.AddDays(-8), UpdatedAt = now.AddDays(-6) },
            // Conversation 2: Customer1 - Caregiver1
            new() { Id = Guid.NewGuid(), ConversationId = Conversation2Id, UserId = Customer1UserId, JoinedAt = now.AddDays(-1), LastReadAt = now.AddHours(-2), UnreadCount = 1, IsMuted = false, CreatedAt = now.AddDays(-1), UpdatedAt = now },
            new() { Id = Guid.NewGuid(), ConversationId = Conversation2Id, UserId = Caregiver1UserId, JoinedAt = now.AddDays(-1), LastReadAt = now.AddHours(-1), UnreadCount = 0, IsMuted = false, CreatedAt = now.AddDays(-1), UpdatedAt = now }
        };
        await context.ConversationParticipants.AddRangeAsync(participants);

        var messages = new List<Message>
        {
            // Conversation 1
            new()
            {
                Id = Guid.NewGuid(), ConversationId = Conversation1Id, SenderId = Customer1UserId,
                Content = "Chào cô Hương, ngày mai cô đến chăm sóc bố tôi nhé. Bố hay uống thuốc muộn, cô nhắc bố giúp nhé.",
                Status = MessageStatus.Read, SentAt = now.AddDays(-8).AddHours(20), ReadAt = now.AddDays(-8).AddHours(20).AddMinutes(5),
                CreatedAt = now.AddDays(-8).AddHours(20), UpdatedAt = now.AddDays(-8).AddHours(20)
            },
            new()
            {
                Id = Guid.NewGuid(), ConversationId = Conversation1Id, SenderId = Caregiver1UserId,
                Content = "Dạ em chào anh. Em sẽ có mặt đúng giờ và nhắc bác uống thuốc ạ. Anh có muốn em đo đường huyết cho bác trước bữa ăn không ạ?",
                Status = MessageStatus.Read, SentAt = now.AddDays(-8).AddHours(20).AddMinutes(10), ReadAt = now.AddDays(-8).AddHours(20).AddMinutes(15),
                CreatedAt = now.AddDays(-8).AddHours(20).AddMinutes(10), UpdatedAt = now.AddDays(-8).AddHours(20).AddMinutes(10)
            },
            new()
            {
                Id = Guid.NewGuid(), ConversationId = Conversation1Id, SenderId = Customer1UserId,
                Content = "Có, cô đo giúp trước ăn sáng và trước ăn trưa nhé. Cảm ơn cô!",
                Status = MessageStatus.Read, SentAt = now.AddDays(-8).AddHours(20).AddMinutes(20), ReadAt = now.AddDays(-8).AddHours(20).AddMinutes(25),
                CreatedAt = now.AddDays(-8).AddHours(20).AddMinutes(20), UpdatedAt = now.AddDays(-8).AddHours(20).AddMinutes(20)
            },
            // Conversation 2
            new()
            {
                Id = Guid.NewGuid(), ConversationId = Conversation2Id, SenderId = Caregiver1UserId,
                Content = "Em đã đến nơi và check-in rồi ạ. Bà Mai hôm nay tinh thần tốt, đã ăn sáng xong.",
                Status = MessageStatus.Read, SentAt = now.Date.AddHours(8).AddMinutes(5), ReadAt = now.Date.AddHours(8).AddMinutes(10),
                CreatedAt = now.Date.AddHours(8).AddMinutes(5), UpdatedAt = now.Date.AddHours(8).AddMinutes(5)
            },
            new()
            {
                Id = Guid.NewGuid(), ConversationId = Conversation2Id, SenderId = Customer1UserId,
                Content = "Cảm ơn cô. Nhắc bà uống thuốc lúc 9h nhé. Bà hay quên.",
                Status = MessageStatus.Read, SentAt = now.Date.AddHours(8).AddMinutes(15), ReadAt = now.Date.AddHours(8).AddMinutes(20),
                CreatedAt = now.Date.AddHours(8).AddMinutes(15), UpdatedAt = now.Date.AddHours(8).AddMinutes(15)
            },
            new()
            {
                Id = Guid.NewGuid(), ConversationId = Conversation2Id, SenderId = Caregiver1UserId,
                Content = "Dạ bà đã uống thuốc rồi ạ. Bây giờ em đang cho bà nghe nhạc và trò chuyện. Bà vui lắm.",
                Status = MessageStatus.Sent, SentAt = now.AddHours(-1),
                CreatedAt = now.AddHours(-1), UpdatedAt = now.AddHours(-1)
            }
        };
        await context.Messages.AddRangeAsync(messages);

        // ==================== 16. MATCHING RESULTS ====================
        var matchingResults = new List<MatchingResult>
        {
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary1Id, CaregiverId = Caregiver1Id,
                OverallScore = 92.5, PersonalityScore = 95, SkillScore = 90,
                AvailabilityScore = 85, LocationScore = 90, PerformanceScore = 96,
                CalculatedAt = now.AddDays(-10), CreatedAt = now.AddDays(-10), UpdatedAt = now.AddDays(-10)
            },
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary1Id, CaregiverId = Caregiver2Id,
                OverallScore = 78.0, PersonalityScore = 70, SkillScore = 85,
                AvailabilityScore = 80, LocationScore = 75, PerformanceScore = 80,
                CalculatedAt = now.AddDays(-10), CreatedAt = now.AddDays(-10), UpdatedAt = now.AddDays(-10)
            },
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary2Id, CaregiverId = Caregiver1Id,
                OverallScore = 88.0, PersonalityScore = 90, SkillScore = 95,
                AvailabilityScore = 80, LocationScore = 90, PerformanceScore = 85,
                CalculatedAt = now.AddDays(-5), CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary3Id, CaregiverId = Caregiver2Id,
                OverallScore = 75.5, PersonalityScore = 72, SkillScore = 80,
                AvailabilityScore = 78, LocationScore = 70, PerformanceScore = 75,
                CalculatedAt = now.AddDays(-2), CreatedAt = now.AddDays(-2), UpdatedAt = now.AddDays(-2)
            }
        };
        await context.MatchingResults.AddRangeAsync(matchingResults);

        // ==================== 17. CARE NOTES (AI Caregiver Assistant) ====================
        var careNotes = new List<CareNote>
        {
            new()
            {
                Id = Guid.NewGuid(), BookingId = Booking1Id, CaregiverId = Caregiver1Id, BeneficiaryId = Beneficiary1Id,
                Observation = "Ông Phúc sáng nay khỏe, đường huyết trước ăn sáng: 120mg/dL. Bác ăn sáng ngon miệng, sau đó chơi cờ tướng với hàng xóm.",
                AssessedMood = MoodLevel.Good, ObservedAt = now.AddDays(-7).Date.AddHours(9),
                AiMoodAnalysis = "Tâm trạng tích cực, hoạt động xã hội tốt. Đường huyết ổn định trong ngưỡng an toàn.",
                SentimentScore = 0.8, RequiresAttention = false, NotifiedCustomer = false,
                CreatedAt = now.AddDays(-7).Date.AddHours(9), UpdatedAt = now.AddDays(-7).Date.AddHours(9)
            },
            new()
            {
                Id = Guid.NewGuid(), BookingId = Booking2Id, CaregiverId = Caregiver1Id, BeneficiaryId = Beneficiary2Id,
                Observation = "Bà Mai hôm nay hơi lú lẫn, quên tên cháu gái. Nhưng sau khi nghe nhạc xưa, bà vui hơn nhiều và hát theo.",
                AssessedMood = MoodLevel.Neutral, ObservedAt = now.Date.AddHours(10),
                AiMoodAnalysis = "Có dấu hiệu suy giảm nhận thức nhẹ. Âm nhạc có tác dụng tích cực. Nên tăng cường hoạt động kích thích trí nhớ.",
                SentimentScore = 0.3, RequiresAttention = true, NotifiedCustomer = true,
                CreatedAt = now.Date.AddHours(10), UpdatedAt = now.Date.AddHours(10)
            }
        };
        await context.CareNotes.AddRangeAsync(careNotes);

        // ==================== 18. ACTIVITY SUGGESTIONS ====================
        var activities = new List<ActivitySuggestion>
        {
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary1Id,
                Title = "Chơi cờ tướng buổi chiều", Description = "Rủ ông Phúc chơi cờ tướng 30 phút, tốt cho tư duy chiến lược và tâm trạng.",
                Category = ActivityCategory.Mental, DurationMinutes = 30, Difficulty = DifficultyLevel.Moderate,
                AiReasoning = "Ông Phúc thích cờ tướng (hobby). Hoạt động trí tuệ giúp duy trì nhận thức.",
                ConfidenceScore = 0.92, IsCompleted = true, CompletedAt = now.AddDays(-7).Date.AddHours(14),
                CaregiverFeedback = "Ông rất vui, chơi say mê", BeneficiaryEngagementRating = 5,
                GeneratedAt = now.AddDays(-7).Date.AddHours(8), ExpiresAt = now.Date,
                CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-7)
            },
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary2Id,
                Title = "Nghe nhạc Trịnh Công Sơn", Description = "Cho bà Mai nghe nhạc Trịnh 20 phút, hỏi bà kỷ niệm liên quan đến bài hát.",
                Category = ActivityCategory.Entertainment, DurationMinutes = 20, Difficulty = DifficultyLevel.VeryEasy,
                AiReasoning = "Bà Mai thích nghe nhạc xưa. Nhạc Trịnh kích thích hồi ức dài hạn, tốt cho bệnh nhân Alzheimer.",
                ConfidenceScore = 0.95, IsCompleted = false,
                GeneratedAt = now.Date.AddHours(8), ExpiresAt = now.AddDays(7),
                CreatedAt = now, UpdatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(), BeneficiaryId = Beneficiary2Id,
                Title = "Tập thể dục nhẹ buổi sáng", Description = "Hướng dẫn bà Mai tập duỗi tay chân 10 phút, vừa tập vừa trò chuyện.",
                Category = ActivityCategory.Physical, DurationMinutes = 10, Difficulty = DifficultyLevel.Easy,
                AiReasoning = "Bà Mai bị loãng xương, vận động nhẹ giúp duy trì sức khỏe xương khớp.",
                ConfidenceScore = 0.88, IsCompleted = false,
                GeneratedAt = now.Date.AddHours(8), ExpiresAt = now.AddDays(7),
                CreatedAt = now, UpdatedAt = now
            }
        };
        await context.ActivitySuggestions.AddRangeAsync(activities);

        // ==================== 19. DAILY REPORTS ====================
        var dailyReports = new List<DailyReport>
        {
            new()
            {
                Id = Guid.NewGuid(), BookingId = Booking1Id, CaregiverId = Caregiver1Id,
                BeneficiaryId = Beneficiary1Id, CustomerId = Customer1Id,
                ReportDate = now.AddDays(-7).Date,
                Summary = "Ông Phúc có ngày tốt. Đường huyết ổn định (120mg/dL sáng, 135mg/dL trưa). Ăn uống tốt, chơi cờ tướng vui vẻ. Tinh thần lạc quan.",
                AverageMood = MoodLevel.Good,
                HealthNotes = "Đường huyết ổn định. Huyết áp 130/80. Không có triệu chứng bất thường.",
                BehaviorNotes = "Hoạt bát, vui vẻ. Chủ động trò chuyện với hàng xóm.",
                AiInsights = "Sức khỏe tổng thể ổn định. Chế độ thuốc đang phát huy tác dụng. Nên duy trì hoạt động xã hội.",
                CaregiverNotes = "Bác rất hợp tác, uống thuốc đúng giờ.",
                CaregiverApproved = true, ApprovedAt = now.AddDays(-7).Date.AddHours(12),
                ViewedByCustomer = true, ViewedAt = now.AddDays(-7).Date.AddHours(18),
                GeneratedAt = now.AddDays(-7).Date.AddHours(12),
                CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-7)
            }
        };
        await context.DailyReports.AddRangeAsync(dailyReports);

        // ==================== SAVE ALL ====================
        await context.SaveChangesAsync();
    }
}
