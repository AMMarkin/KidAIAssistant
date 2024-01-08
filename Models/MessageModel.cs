using System.Text.Json.Serialization;

namespace TheOneWhoWritesYourSpeech.Models;

/// <summary>
/// Модель сообщения для ChatGPT
/// </summary>
public class MessageModel
{
    /// <summary>
    /// От кого сообщения
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}
