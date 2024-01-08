using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using TheOneWhoWritesYourSpeech.Exceptions;
using TheOneWhoWritesYourSpeech.Providers;
using TheOneWhoWritesYourSpeech.Models;

namespace TheOneWhoWritesYourSpeech.Services;

/// <summary>
/// Класс GptService предназначен для взаимодействия с сервисом GPT (OpenAI), позволяя отправлять запросы и получать ответы.
/// </summary>
public class GptService(
        HttpClient http,
        OpenAiDataProvider openAiData
        )
{
    ///<summary>
    /// Сообщение, задающее контекст работы модели
    ///</summary>
    private readonly MessageModel promt = new()
    {
        Role = "system",
        Content = "Веди себя как учитель начальной школы, помогая ребенку познавать мир и заниматься своими хобби. " +
                      "Твои ответы должны быть познавательными, поучительными и понятными для 9-летнего ребенка."
        //Content = "Веди себя как учитель физики. Ты должен быть строгим и строго реагировать на глупые вопросы студентов."
    };

    /// <summary>
    /// Метод для отправки вопроса в сервис GPT и получения ответа.
    /// </summary>
    /// <param name="speech">Текст вопроса, который будет отправлен сервису.</param>
    /// <returns>Строка с ответом от сервиса GPT.</returns>
    /// <exception cref="ChatGPTException">Исключение, которое выбрасывается, если от сервиса не получен успешный ответ.</exception>
    internal async Task<string> Ask(string speech)
    {
        string url = @"https://api.openai.com/v1/chat/completions";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {openAiData.ApiKey}");

        var gptRequest = new ChatGTPRequestModel()
        {
            Messages = new List<MessageModel>()
            {
                promt,
                new MessageModel()
                {
                    Role="user",
                    Content = speech
                }
            }
        };

        request.Content = JsonContent.Create<ChatGTPRequestModel>(gptRequest);

        //отправляем запрос
        var response = await http.SendAsync(request);


        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new ChatGPTException($"{response.ReasonPhrase}\n{content}");

        return JObject.Parse(content)["choices"]![0]!["message"]!["content"]!.ToString();
    }

    internal async Task<string> AskDallePrompt(string text)
    {
        string url = @"https://api.openai.com/v1/chat/completions";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {openAiData.ApiKey}");

        var gptRequest = new ChatGTPRequestModel()
        {
            Messages = new List<MessageModel>()
            {
                new MessageModel()
                {
                    Role="user",
                    Content = "Преобразуй следуйющий текст в описание иллюстрации для dall-e-2 " +
                    "(иллюстрация должна быть как в энциклопедии: не страшной, но достаточно реалистичной): " +
                    text
                }
            }
        };

        request.Content = JsonContent.Create<ChatGTPRequestModel>(gptRequest);

        //отправляем запрос
        var response = await http.SendAsync(request);


        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new ChatGPTException($"{response.ReasonPhrase}\n{content}");

        return JObject.Parse(content)["choices"]![0]!["message"]!["content"]!.ToString();
    }


}
