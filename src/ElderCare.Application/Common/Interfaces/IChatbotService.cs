using ElderCare.Application.Features.Profiles.DTOs;

namespace ElderCare.Application.Common.Interfaces;

public interface IChatbotService
{
    /// <summary>
    /// Send a message to the AI chatbot with beneficiary context
    /// </summary>
    Task<ChatbotResponse> AskAsync(Guid userId, ChatbotRequest request, CancellationToken ct = default);
}

public class ChatbotRequest
{
    public string Message { get; set; } = string.Empty;
    public Guid? BeneficiaryId { get; set; }
    public List<ChatMessage>? History { get; set; }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
}

public class ChatbotResponse
{
    public string Reply { get; set; } = string.Empty;
    public List<CaregiverSuggestion>? Suggestions { get; set; }
}

public class CaregiverSuggestion
{
    public Guid CaregiverId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double MatchScore { get; set; }
    public string Reason { get; set; } = string.Empty;
}
