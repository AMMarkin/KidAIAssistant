using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TheOneWhoWritesYourSpeech.Exceptions;
using TheOneWhoWritesYourSpeech.Extensions;
using TheOneWhoWritesYourSpeech.Providers;
using TheOneWhoWritesYourSpeech.Services;

namespace TheOneWhoWritesYourSpeech;

public class Bot(
        BotDataProvider botData,
        FileService fileService,
        SpeechService speechService,
        GptService gptService,
        ImageService imageService
    )
{

    private const string errorMessage = "Что-то я приболел... Напишите, пожалуйста моему папе @random_symbols";

    /// <summary>
    /// Запускает бота
    /// </summary>
    public string Start()
    {
        var bot = new TelegramBotClient(botData.Token);

        bot.StartReceiving(
            HandleUpdateAsync,
            HandleExceptionAsync,
            receiverOptions: new ReceiverOptions()
            {
                AllowedUpdates = new[] { UpdateType.Message }
            },
            new CancellationTokenSource().Token
        );

        return "Бот запущен...";
    }

    //обработка апдейтов
    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var message = update.Message;
        var fileName = string.Empty;

        if (message is null)
            return;

        //ничего не написали => кинули стикер, файл и тд
        if (!message.HasVoice() && !(message.IsStartCommand() || message.HasText()))
        {
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                "Я еще маленький бот и только научился читать. \nЗапишите голосовое сообщение, их я уже умею слушать или просто напишите свой вопрос",
                cancellationToken: ct);
            return;
        }

        try
        {
            //ставим статус "печатает..."
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing, cancellationToken: ct);

            string question;

            if (message.HasVoice())
            {
                //скачиваем файл
                fileName = await fileService.Download(message.Voice!.FileId, bot, ct);

                //распознаем речь в голосовом сообщении
                question = await speechService.Transcrit(fileName);
            }
            else
            {
                question = message.Text!;
            }

            //отправляем вопрос в GPT
            var answer = await gptService.Ask(question);

            if (botData.EnabledImages)
            {
                //рисуем иллюстрацию
                var imageUrl = await imageService.CreateIllustration(answer);

                //отправляем ответ пользователю
                await bot.SendPhotoAsync(message.Chat.Id, new InputFileUrl(imageUrl), caption: answer, cancellationToken: ct);
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id, answer, cancellationToken: ct);
            }
        }
        catch (SpeechTranclationException ex)
        {
            Console.WriteLine($"\tОшибка распознавания речи:\n{ex.Message}");
            await bot.SendTextMessageAsync(message.Chat.Id, errorMessage, cancellationToken: ct);
        }
        catch (NotRecognizedSpeechException)
        {
            await bot.SendTextMessageAsync(message.Chat.Id, "Извини, я тебя не понял", cancellationToken: ct);
        }
        catch (ChatGPTException ex)
        {
            Console.WriteLine($"\tПроизошла ошибка в чате:\n{ex.Message}");
            await bot.SendTextMessageAsync(message.Chat.Id, errorMessage, cancellationToken: ct);
        }
        catch (GeneratingImageException ex)
        {
            Console.WriteLine($"\tПроизошла ошибка генерации изображения:\n{ex.Message}");
            await bot.SendTextMessageAsync(message.Chat.Id, errorMessage, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\tПроизошла ошибка:\n{ex.Message}\n{ex.StackTrace}");
            await bot.SendTextMessageAsync(message.Chat.Id, errorMessage, cancellationToken: ct);
        }
        finally
        {
            //удаляем файл если он был создан
            if (!string.IsNullOrEmpty(fileName))
                fileService.Delete(fileName);
        }
    }

    //обработка ошибок
    private Task HandleExceptionAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        var serializeOptions = new JsonSerializerOptions() { WriteIndented = true };
        Console.WriteLine(JsonSerializer.Serialize(exception, serializeOptions));

        return Task.CompletedTask;
    }
}
