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
string? text = "";
while (text.ToLower() != "exit")
{
    text = Console.ReadLine();
    if (text is null)
    {
        text = "";
    }
}

async Task OnMessage(Message msg, UpdateType type)
{
    string welcomeText =
@"🎓 Привіт! Це Schedlify — твій особистий гід у світі університетських розкладів!

Ми створили цього бота, щоб зробити твоє студентське життя трішки простішим:
✅ знаходь розклад за секунди
✅ отримуй сповіщення про пари й зміни
✅ обирай групу, яка тобі потрібна

А тепер зроби перший крок 🎯
👇 Натисни кнопку нижче щоб знайти свою групу:";

    ApiClient apiClient = new ApiClient();
    switch (msg.Text)
    {
        case "/start":
            await botClient.SendMessage(
                msg.Chat,
                welcomeText,
                replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { new InlineKeyboardButton("🔘 Виберіть ваш університет", "choose_university,0") })
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
                List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
                buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("🔽Приховати", "hideMessage") });
                await botClient.SendMessage(
                    msg.Chat,
                    message,
                    replyMarkup: new InlineKeyboardMarkup(buttonList)
                );
            }
            break;

        case "/show_schedule":
            if (msg.From is not null)
            {
                startMenu.ShowSchedule(msg.From.Id, DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"))));

            }
            break;
    }

}

async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query } and)
    {
        await botClient.AnswerCallbackQuery(query.Id, query.Data);

        ApiClient apiClient = new ApiClient();
        switch (query.Data.Split(',')[0])
        {
            case "choose_university":

                startMenu.ShowUniversityChooseList(query.From.Id, int.Parse(query.Data.Split(',')[1]));
               
                break;

            case "chosen_university":
                startMenu.ShowDepartmentChooseList(query.From.Id, int.Parse(query.Data.Split(',')[1]), int.Parse(query.Data.Split(',')[2]));
                break;

            case "chosen_department":
                startMenu.ShowGroupChooseList(query.From.Id, int.Parse(query.Data.Split(',')[1]), int.Parse(query.Data.Split(',')[2]));
                break;

            case "chosen_group":
                startMenu.ShowGroupChosen(query.From.Id, int.Parse(query.Data.Split(',')[1]));
                break;

            case "subscribe":
                if (query.From is not null)
                {
                    TgUser user = await apiClient.PostAsync<TgUser>(
                    "/change_subscription_status",
                    query.From.Id,
                    new Dictionary<string, object>()
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
                        query.From.Id,
                        message
                    );
                }
                break;

            case "show":

                startMenu.ShowSchedule(query.From.Id, DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"))).AddDays(int.Parse(query.Data.Split(',')[1])));
                break;

            case "showAssignmentInfo":

                startMenu.ShowAssignmentInfo(query.From.Id, int.Parse(query.Data.Split(',')[1]), int.Parse(query.Data.Split(',')[2]));
                break;

            case "hideMessage":
                await botClient.DeleteMessage(query.From.Id, query.Message.Id);
               
                break;
        }
       
        
        await botClient.DeleteMessage(query.From.Id, query.Message.MessageId);
    }
}


