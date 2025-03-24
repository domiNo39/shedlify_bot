using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using dotenv.net;

//отримуємо токен
DotEnv.Load();
var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
if (string.IsNullOrEmpty(botToken))
{
    Console.WriteLine("Відсутній токен у .env файлі");
    Environment.Exit(1);
}

// Створюємо бота
var botClient = new TelegramBotClient(botToken);
StartMenu startMenu = new StartMenu(botClient);
botClient.OnMessage += OnMessage;
botClient.OnUpdate += OnUpdate;

using var cts = new CancellationTokenSource();

var me = await botClient.GetMe();
Console.WriteLine($"Бот @{me.Username} запущений!");
Console.ReadLine(); // Блокуючий виклик, щоб бот працював

// method that handle messages received by the bot:
async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/start")
    {
        await botClient.SendMessage(msg.Chat, "Welcome! Pick one direction",
            replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { new InlineKeyboardButton("обрати групу", "choose_university") }));

    }
}

// method that handle other types of updates received by the bot:
async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query } and) // non-null CallbackQuery
    {
        switch (query.Data.Split(',')[0])
        {
            case "choose_university":
                startMenu.ShowUniversityChooseList(query.From.Id);
                break;

            case "chosen_university":

                break;
        }
        //await botClient.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
        //await botClient.SendMessage(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
        await botClient.AnswerCallbackQuery(query.Id);


    }
}