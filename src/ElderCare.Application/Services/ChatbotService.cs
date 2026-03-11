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
        // Lấy từ User Secrets (Local) hoặc Environment Variables (Azure)
        _apiKey = configuration["Gemini:ApiKey"] ?? "";
        _model = configuration["Gemini:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<ChatbotResponse> AskAsync(Guid userId, ChatbotRequest request, CancellationToken ct = default)
    {
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
- Tình trạng sức khỏe: {beneficiary.MedicalConditions ?? "Không có"}
- Khả năng vận động: {beneficiary.MobilityLevel}
- Nhu cầu đặc biệt: {beneficiary.SpecialNeeds ?? "Không có"}";
            }
        }

        var systemPrompt = $@"Bạn là trợ lý AI của nền tảng Tuổi Vàng...

QUY TẮC TRÌNH BÀY:
- TUYỆT ĐỐI KHÔNG sử dụng các ký tự Markdown như dấu sao (*), dấu thăng (#) hoặc gạch đầu dòng.
- Để phân tách các ý, hãy sử dụng số thứ tự (1., 2., 3.) hoặc xuống dòng hai lần.
- Không in đậm, không in nghiêng.
- Trả lời dưới dạng văn bản thuần túy (Plain Text).
{beneficiaryContext}";

        var contents = new List<object>();

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

        contents.Add(new { role = "user", parts = new[] { new { text = request.Message } } });

        try
        {
            var requestBody = new
            {
                system_instruction = new { parts = new[] { new { text = systemPrompt } } },
                contents,
                generationConfig = new
                {
                    maxOutputTokens = 1500, // Tăng lên để tránh bị ngắt quãng
                    temperature = 0.7,
                    topP = 0.95 // Thêm cái này để câu trả lời mượt hơn
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            // --- PHẦN REFACTOR BẢO MẬT ---
            // 1. URL sạch, không chứa API Key
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // 2. Truyền API Key qua Header thay vì Query Parameter
            httpRequest.Headers.Add("x-goog-api-key", _apiKey);

            var response = await _httpClient.SendAsync(httpRequest, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                // 3. Log lỗi nhưng không làm lộ URL (phòng trường hợp cấu hình log tự động ghi URL)
                _logger.LogError("Gemini API error: {StatusCode} - {Body}", response.StatusCode, errorBody);

                return new ChatbotResponse { Reply = "Xin lỗi, tôi đang bận một chút. Thử lại sau nhé!" };
            }

            var responseJson = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(responseJson);
            var reply = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";

            return new ChatbotResponse { Reply = reply };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChatbotService Exception");
            return new ChatbotResponse { Reply = "Có lỗi xảy ra, nhóm phát triển đang kiểm tra." };
        }
    }
}