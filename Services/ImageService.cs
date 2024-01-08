using Newtonsoft.Json.Linq;
using TheOneWhoWritesYourSpeech.Exceptions;
using TheOneWhoWritesYourSpeech.Models;
using TheOneWhoWritesYourSpeech.Providers;

namespace TheOneWhoWritesYourSpeech.Services;

public class ImageService(HttpClient http, GptService gptService,  OpenAiDataProvider openAiData)
{


    internal async Task<string> CreateIllustration(string text)
    {
        var prompt = await gptService.AskDallePrompt(text);

        string url = @"https://api.openai.com/v1/images/generations";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {openAiData.ApiKey}");

        var dalleRequest = new DalleRequestModel()
        {
            Prompt = prompt
        };

        request.Content = JsonContent.Create<DalleRequestModel>(dalleRequest);

        //отправляем запрос
        var response = await http.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new GeneratingImageException($"{response.ReasonPhrase}\n{content}");

        var imageUrl = JObject.Parse(content)["data"]![0]!["url"]!.ToString();

        return imageUrl;
    }
}
