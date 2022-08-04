using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

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

        if (message is null || message.Text is null)
        {
            throw new Exception("Message was null");
        }

        if (message.Text.ToLower() == "/start")
        {
            await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
            return;
        }
        await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
    }
}

static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    // Некоторые действия
    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
}

