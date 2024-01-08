using Newtonsoft.Json.Linq;
using TheOneWhoWritesYourSpeech.Exceptions;
using TheOneWhoWritesYourSpeech.Providers;

namespace TheOneWhoWritesYourSpeech.Services;

/// <summary>
/// Класс для распознавания речи в аудиофайлах используя OpenAI
/// </summary>
/// <param name="http"></param>
/// <param name="openAiData"></param>
public class SpeechService(
    HttpClient http,
    OpenAiDataProvider openAiData
    )
{

    /// <summary>
    /// Асинхронно распознает речь в аудиофайле используя API OpenAI.
    /// </summary>
    /// <param name="fileName">Путь к аудиофайлу, который требуется транскрибировать.</param>
    /// <returns>Строка, содержащая текст, полученный в результате распознавания аудиофайла.</returns>
    /// <exception cref="SpeechTranclationException">Исключение выбрасывается, если API возвращает ошибку при транскрипции.</exception>
    /// <exception cref="NotRecognizedSpeechException">Исключение выбрасывается, если речь в аудиофайле не была распознана.</exception>
    internal async Task<string> Transcrit(string fileName)
    {
        //составляем запрос
        string url = @"https://api.openai.com/v1/audio/transcriptions";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {openAiData.ApiKey}");
        request.Content = new MultipartFormDataContent()
        {
            {new ByteArrayContent(File.ReadAllBytes(fileName)), "file", fileName },
            {new StringContent("whisper-1"), "model" }
        };

        //отправляем запрос
        var response = await http.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new SpeechTranclationException($"{response.ReasonPhrase}\n{content}");

        var speech = JObject.Parse(content)["text"]?.ToString()!;

        if (string.IsNullOrWhiteSpace(speech))
            throw new NotRecognizedSpeechException();

        return speech;
    }

}
