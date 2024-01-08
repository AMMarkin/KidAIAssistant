using System.Text.Json.Serialization;

namespace TheOneWhoWritesYourSpeech.Models;

/// <summary>
/// Модель запроса к ChatGPT
/// </summary>
public class ChatGTPRequestModel
{
    /// <summary>
    /// ID модели
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = "gpt-3.5-turbo-1106";

    /// <summary>
    /// Сообщения для модели
    /// </summary>
    [JsonPropertyName("messages")]
    public required IEnumerable<MessageModel> Messages { get; set; }

}
