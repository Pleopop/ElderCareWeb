using System.Text;
using System.Text.Json;
using ElderCare.Application.Common.Interfaces;
using ElderCare.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElderCare.Application.Services;

public class ChatbotService : IChatbotService
{
    private readonly HttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChatbotService> _logger;
    private readonly string _apiKey;
    private readonly string _model;

    public ChatbotService(
        HttpClient httpClient,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<ChatbotService> logger)
    {
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _apiKey = configuration["Gemini:ApiKey"] ?? "";
        _model = configuration["Gemini:Model"] ?? "gemini-2.0-flash";
    }

    public async Task<ChatbotResponse> AskAsync(Guid userId, ChatbotRequest request, CancellationToken ct = default)
    {
        // Build beneficiary context if provided
        string beneficiaryContext = "";
        if (request.BeneficiaryId.HasValue)
        {
            var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(request.BeneficiaryId.Value, ct);
            if (beneficiary != null)
            {
                var age = DateTime.Today.Year - beneficiary.DateOfBirth.Year;
                beneficiaryContext = $@"

THÔNG TIN NGƯỜI THỤ HƯỞNG:
- Họ tên: {beneficiary.FullName}
- Tuổi: {age}
- Giới tính: {beneficiary.Gender}
- Địa chỉ: {beneficiary.Address ?? "Chưa cung cấp"}
- Tình trạng sức khỏe: {beneficiary.MedicalConditions ?? "Không có"}
- Thuốc đang dùng: {beneficiary.Medications ?? "Không có"}
- Dị ứng: {beneficiary.Allergies ?? "Không có"}
- Khả năng vận động: {beneficiary.MobilityLevel}
- Tình trạng nhận thức: {beneficiary.CognitiveStatus}
- Nhu cầu đặc biệt: {beneficiary.SpecialNeeds ?? "Không có"}
- Tính cách: {beneficiary.PersonalityTraits ?? "Chưa cung cấp"}
- Sở thích: {beneficiary.Hobbies ?? "Chưa cung cấp"}";
            }
        }

        var systemPrompt = $@"Bạn là trợ lý AI của nền tảng Tuổi Vàng — dịch vụ chăm sóc người cao tuổi tại Việt Nam.
Vai trò của bạn:
1. Tư vấn dịch vụ chăm sóc phù hợp cho người thụ hưởng
2. Đề xuất loại caregiver phù hợp dựa trên tình trạng sức khỏe
3. Giải đáp thắc mắc về quy trình đặt dịch vụ, thanh toán, đánh giá
4. Hỗ trợ thông tin về chăm sóc sức khỏe người cao tuổi

Quy tắc:
- Trả lời bằng tiếng Việt, thân thiện và dễ hiểu
- Dựa trên thông tin người thụ hưởng nếu có để tư vấn cá nhân hóa
- Không tư vấn y khoa chuyên sâu, khuyên người dùng gặp bác sĩ khi cần
- Giữ câu trả lời ngắn gọn, rõ ràng (tối đa 300 từ)
{beneficiaryContext}";

        // Build Gemini API contents array
        var contents = new List<object>();

        // Add conversation history
        if (request.History != null)
        {
            foreach (var msg in request.History.TakeLast(10))
            {
                contents.Add(new
                {
                    role = msg.Role == "assistant" ? "model" : "user",
                    parts = new[] { new { text = msg.Content } }
                });
            }
        }

        // Add current user message
        contents.Add(new
        {
            role = "user",
            parts = new[] { new { text = request.Message } }
        });

        try
        {
            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents,
                generationConfig = new
                {
                    maxOutputTokens = 800,
                    temperature = 0.7
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Gemini API endpoint: key is passed as query parameter
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(url, httpContent, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Gemini API error: {StatusCode} - {Body}", response.StatusCode, errorBody);

                return new ChatbotResponse
                {
                    Reply = "Xin lỗi, tôi đang gặp sự cố kỹ thuật. Vui lòng thử lại sau hoặc liên hệ hỗ trợ."
                };
            }

            var responseJson = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(responseJson);

            // Gemini response: candidates[0].content.parts[0].text
            var reply = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";

            return new ChatbotResponse { Reply = reply };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChatbotService error");
            return new ChatbotResponse
            {
                Reply = "Xin lỗi, tôi đang gặp sự cố. Vui lòng thử lại sau."
            };
        }
    }
}
