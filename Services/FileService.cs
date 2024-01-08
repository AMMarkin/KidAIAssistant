using Telegram.Bot;

namespace TheOneWhoWritesYourSpeech.Services;

// <summary>
/// Класс для работы с файловой системой, обеспечивающий скачивание и удаление файлов.
/// </summary>
public class FileService
{
    /// <summary>
    /// Асинхронно скачивает файл, используя идентификатор и клиент Telegram-бота.
    /// </summary>
    /// <param name="fileId">Идентификатор файла для скачивания.</param>
    /// <param name="bot">Клиент Telegram бота, используемый для скачивания файла.</param>
    /// <param name="ct">Токен отмены операции для контроля прерывания загрузки.</param>
    /// <returns>Путь к скачанному файлу.</returns>
    internal async Task<string> Download(string fileId, ITelegramBotClient bot, CancellationToken ct)
    {
        string dir = Directory.GetCurrentDirectory() + "\\voices\\";

        if(!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var fileName = $"{dir}{fileId}.ogg";

        using (var stream = new FileStream(fileName, FileMode.Create))
        {
            await bot.GetInfoAndDownloadFileAsync(fileId, stream, ct);
        }

        return fileName;
    }

    /// <summary>
    /// Удаляет файл по указанному пути.
    /// </summary>
    /// <param name="fileName">Путь к файлу, который необходимо удалить.</param>
    internal void Delete(string fileName) => File.Delete(fileName);
}
