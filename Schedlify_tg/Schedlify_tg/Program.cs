using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using dotenv.net;

DotEnv.Load(new DotEnvOptions(true, ["../../../.env"]));
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
    ApiClient apiClient = new ApiClient();
    switch (msg.Text)
    {
        case "/start":
            await botClient.SendMessage(
                msg.Chat,
                "Привіт, я чат-бот Schedlify\nЩоб розпочати роботу оберіть вашу групу",
                replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { new InlineKeyboardButton("Обрати університет", "choose_university") })
            );
            if (msg.From is not null)
            {
                await apiClient.PostAsync<TgUser>(
                "/tgusers",
                msg.From.Id, new TgUserBase
                {
                    FirstName = msg.From.FirstName,
                    LastName = msg.From.LastName,
                    Username = msg.From.Username
                }
                );
            }
            
            break;
        case "/subscribe":
            if (msg.From is not null)
            {
                TgUser user = await apiClient.PostAsync<TgUser>(
                "/change_subscription_status",
                msg.From.Id,
                null
                );
                string message;
                if (user.Subscribed)
                {
                    message = "Ви успішно підписались";
                }
                else
                {
                    message = "Ви успішно відписались";
                }
                await botClient.SendMessage(
                    msg.Chat,
                    message
                );
            }
            break;

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

            case "chosen_department":
                startMenu.ShowGroupChooseList(query.From.Id, int.Parse(query.Data.Split(',')[1]));
                break;

            case "chosen_group":
                startMenu.ShowGroupChosen(query.From.Id, int.Parse(query.Data.Split(',')[1]));
                break;
        }
        await botClient.AnswerCallbackQuery(query.Id,query.Data);


    }
}


