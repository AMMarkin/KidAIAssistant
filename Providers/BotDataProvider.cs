namespace TheOneWhoWritesYourSpeech.Providers;

/// <summary>
/// Поставщик информации для Telegram-бота
/// </summary>
public class BotDataProvider
{
    /// <summary>
    /// Токен бота
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Генерирует ли бот картинки
    /// </summary>
    public bool EnabledImages { get; set; }
}
