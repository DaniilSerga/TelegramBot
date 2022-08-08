using Bot.BusinessLogic.Services.Implementations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using System.IO;
using VideoLibrary;

ITelegramBotClient bot = new TelegramBotClient("5499122271:AAGRTlxr4IV_3WJI82Kci97MpIQ3GjM2los");

Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }, // receive all update types
};
bot.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken
);
Console.ReadLine();

static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Некоторые действия
    Console.ResetColor();
    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
    if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
    {
        var message = update.Message;

        if (update is null)
        {
            return;
        }

        if (message is null || message.Text is null)
        {
            return;
        }

        if (message.Text.ToLower() == "/start")
        {
            await botClient.SendTextMessageAsync(message.Chat, "Начнём. Отправьте ссылку на youtube видео, которое вы хотите конвертировать в аудио:");
            Console.WriteLine("/start");
        }
        else if (message.Text.StartsWith("https://www.youtube.com/") || message.Text.StartsWith("https://youtu.be/") && update.Message is not null)
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Песня загружается\nЭто может занять некоторое время...");

            string songPath = new ConversionsService().Convert(message.Text, update.Message.Chat.Id, update.Message.Chat.FirstName + " " + update.Message.Chat.LastName);

            if (string.IsNullOrEmpty(songPath))
            {
                throw new Exception("Bad link");
            }

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ваш трек:");

            using (var stream = System.IO.File.OpenRead(songPath))
            {
                _ = await botClient.SendAudioAsync(update.Message.Chat.Id,
                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, songPath.Substring(songPath.LastIndexOf('\\'), songPath.Length - songPath.LastIndexOf('\\'))),
                cancellationToken: cancellationToken);

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Отправьте следующую ссылку:");
            }

            Console.WriteLine("-------------------\nSENDED SUCCESFULLY|\n-------------------");
        }
        else
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Неизвестная команда, повторите попытку:");
            Console.WriteLine("Неизвестная команда от пользователя");
        }
    }
}

static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    // Некоторые действия
    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
}

