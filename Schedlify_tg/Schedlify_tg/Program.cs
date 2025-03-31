using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using dotenv.net;

DotEnv.Load(new DotEnvOptions(true,new List<string> { "../../../.env"}));
var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
if (string.IsNullOrEmpty(botToken))
{
    Console.WriteLine("Відсутній токен у .env файлі");
    Environment.Exit(1);
}

var botClient = new TelegramBotClient(botToken);
StartMenu startMenu = new StartMenu(botClient);
botClient.OnMessage += OnMessage;
botClient.OnUpdate += OnUpdate;

using var cts = new CancellationTokenSource();

var me = await botClient.GetMe();
Console.WriteLine($"Бот @{me.Username} запущений!");
Console.ReadLine(); 

async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/start")
    {
        await botClient.SendMessage(msg.Chat, "Welcome! Pick one direction",
            replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { new InlineKeyboardButton("обрати групу", "choose_university") }));

    }
}

async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query } and)
    {
        switch (query.Data.Split(',')[0])
        {
            case "choose_university":
                startMenu.ShowUniversityChooseList(query.From.Id);
                break;

            case "chosen_university":
                startMenu.ShowDepartmentChooseList(query.From.Id, int.Parse(query.Data.Split(',')[1]));
                break;
        }
        await botClient.AnswerCallbackQuery(query.Id);


    }
}
