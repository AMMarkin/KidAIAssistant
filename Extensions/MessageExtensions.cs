using Telegram.Bot.Types;

namespace TheOneWhoWritesYourSpeech.Extensions;

/// <summary>
/// Статический класс расширения, предоставляющий методы расширения для объектов типа <see cref="Message"/>.
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    /// Проверяет, содержит ли сообщение голосовое сообщение.
    /// </summary>
    /// <param name="message">Объект сообщения для проверки.</param>
    /// <returns>Возвращает <see langword="true"/>, если сообщение содержит голосовое сообщение; в противном случае возвращает <see langword="false"/>.</returns>
    public static bool HasVoice(this Message message) => message.Voice is not null;

    /// <summary>
    /// Проверяет, содержит ли сообщение текст.
    /// </summary>
    /// <param name="message">Объект сообщения для проверки.</param>
    /// <returns>Возвращает <see langword="true"/>, если сообщение содержит текст; в противном случае возвращает <see langword="false"/>.</returns>
    public static bool HasText(this Message message) => message.Text is not null;

    /// <summary>
    /// Проверяет, является ли текст сообщения командой <c>'/start'</c>.
    /// </summary>
    /// <param name="message">Объект сообщения для проверки.</param>
    /// <returns>Возвращает <see langword="true"/>, если текст сообщения соответствует команде <c>'/start'</c>; в противном случае возвращает <see langword="false"/>.</returns>
    public static bool IsStartCommand(this Message message) => message.Text is "/start";
}
