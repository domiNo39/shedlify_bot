// Bot.cs
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using dotenv.net;
using Telegram.Bot.Types.Enums;

namespace Schedlify_tg
{
    public class Bot
    {
        private TelegramBotClient _botClient;
        private CancellationTokenSource _cts;

        public Bot()
        {
            // Load environment variables
            DotEnv.Load();
            var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine("Telegram Bot Token not found in .env file.");
                Environment.Exit(1); // Exit if token is not found
            }

            _botClient = new TelegramBotClient(botToken);
            _cts = new CancellationTokenSource();
        }

        public async Task StartReceiving()
        {
            // Start receiving messages
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // Receive all update types
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );

            var me = await _botClient.GetMe();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();

            // Send cancellation request to stop receiving
            _cts.Cancel();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Echo received message text
            Message sentMessage = await botClient.SendMessage(
                chatId: chatId,
                text: "Hello World!",
                cancellationToken: cancellationToken);

            Console.WriteLine($"Echoed message '{sentMessage.Text}' to chat {chatId}.");
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}