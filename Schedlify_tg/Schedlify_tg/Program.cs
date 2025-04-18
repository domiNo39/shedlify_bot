﻿using Telegram.Bot;
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
                replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { new InlineKeyboardButton("Обрати університет", "choose_university,0") })
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

        case "/show_schedule":
            if (msg.From is not null)
            {
                startMenu.ShowSchedule(msg.From.Id, DateOnly.FromDateTime(DateTime.Now));

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

                startMenu.ShowSchedule(query.From.Id, DateOnly.FromDateTime(DateTime.Now).AddDays(int.Parse(query.Data.Split(',')[1])));
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


