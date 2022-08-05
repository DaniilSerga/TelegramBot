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
            await botClient.SendTextMessageAsync(message.Chat, "Начнём. Ожидаю ссылку на youtube видео:");
        }

        if (message.Text.StartsWith("https://www.youtube.com/"))
        {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, "Песня загружается...");

            string songPath = new ConversionsService().Convert(message.Text, update.Message.Chat.Id, update.Message.Chat.FirstName + " " + update.Message.Chat.LastName);

            await using var stream = System.IO.File.OpenRead(songPath);
            _ = botClient.SendAudioAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), cancellationToken: cancellationToken);
        }
    }
}

static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    // Некоторые действия
    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
}

