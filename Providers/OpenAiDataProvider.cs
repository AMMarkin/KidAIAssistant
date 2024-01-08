namespace TheOneWhoWritesYourSpeech.Providers;

/// <summary>
/// Поставщик информации для OpenAI
/// </summary>
public class OpenAiDataProvider
{
    /// <summary>
    /// Ключ API OpenAI
    /// </summary>
    public required string ApiKey { get; set; }
}
