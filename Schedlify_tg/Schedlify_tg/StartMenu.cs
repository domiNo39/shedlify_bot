using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

public class StartMenu
{
    private readonly ITelegramBotClient _botClient;
    private ApiClient _apiClient;

    public StartMenu(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _apiClient = new ApiClient();
  
    }

    public async void ShowUniversityChooseList(long userId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        List<University> _universities = await _apiClient.GetAsync<List<University>>("/universities", userId);
        foreach (University uni in _universities)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(uni.Name, $"chosen_university,{uni.Id}") });

        }

        await _botClient.SendMessage(userId, "Виберіть університет", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }
    public async void ShowDepartmentChooseList(long userId, int universityId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        Dictionary<string, string> _params = new Dictionary<string, string>();
        _params.Add("universityId", $"{universityId}");
        List<Department> _departments = await _apiClient.GetAsync<List<Department>>("/departments", userId, _params);
        foreach (Department dep in _departments)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(dep.Name, $"chosen_department,{dep.Id}") });

        }

        await _botClient.SendMessage(userId, "Виберіть факультет", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowGroupChooseList(long userId, int departmentId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        Dictionary<string, string> _params = new Dictionary<string, string>();
        _params.Add("departmentId", $"{departmentId}" );
        List<Group> _groups = await _apiClient.GetAsync<List<Group>>("/groups", userId, _params);
        foreach (Group group in _groups)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(group.Name, $"chosen_group,{group.Id}") });

        }

        await _botClient.SendMessage(userId, "Виберіть групу", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowGroupChosen(long userId, int groupId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        
        Group group = await _apiClient.GetAsync<Group>($"/groups/{groupId}", userId);
        TgUser user = await _apiClient.PostAsync<TgUser>($"/tgusers/groups/{groupId}", userId, new Dictionary<string, string>());
        buttonList.Add(new List<InlineKeyboardButton> {
            new InlineKeyboardButton("Підписатись", "subscribe"),
            new InlineKeyboardButton("Переглянути розклад", "show,0")});

        await _botClient.SendMessage(userId, $"Ви обрали групу {group.Name}", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowSchedule(long userId, DateOnly date)
    {
        DateTime dateTime = DateTime.Now;
        int a = DateOnly.FromDateTime(dateTime).DayNumber - date.DayNumber;
        List<Assignment> assignmentList = new List<Assignment>();
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();

        foreach (Assignment assignment in assignmentList)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton($"{assignment.StartTime}: {assignment.ClassId}", $"show,{assignment.Id}") });

        }

        buttonList.Add(new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton("<-", $"show,{a-1}"),
            new InlineKeyboardButton("->", $"show,{a+1}")
        });
        await _botClient.SendMessage(userId, $"Розклад на ", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }
}

