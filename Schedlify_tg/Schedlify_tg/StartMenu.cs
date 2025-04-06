using System;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

public class StartMenu
{
    private readonly ITelegramBotClient _botClient;
    private ApiClient _apiClient;
    private readonly int defaultLimit = 8;

    public Dictionary<Mode, string> Modes = new Dictionary<Mode, string>()
    {
        {Mode.Online, "Дистанційно"},
        {Mode.Offline, "Очно" }
    };

    public StartMenu(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _apiClient = new ApiClient();
  
    }

    public async void ShowUniversityChooseList(long userId, int i)
    {
        
        List <List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        List<University> _universities = await _apiClient.GetAsync<List<University>>($"/universities?offset={defaultLimit*i}&limit={defaultLimit}", userId);
        foreach (University uni in _universities)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(uni.Name, $"chosen_university,{uni.Id}") });

        }

        List<InlineKeyboardButton> buttonNavigation = new List<InlineKeyboardButton>();
        if (i > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("<-", $"choose_university,{i - 1}"));
        }
        if ((await _apiClient.GetAsync<List<University>>($"/universities?offset={defaultLimit * (i+1)}&limit={defaultLimit}", userId)).Count > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("->", $"choose_university,{i + 1}"));
        }
        buttonList.Add(buttonNavigation);
        buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("Сховати", "hideMessage") });

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
        buttonList.Add(new List<InlineKeyboardButton> { 
            new InlineKeyboardButton("Назад", $"choose_university,0"),
            new InlineKeyboardButton("Сховати", "hideMessage")
        });

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

        //University uni = await _apiClient.GetAsync<University>($"/universities/{departmentId}", userId);
        //buttonList.Add(new List<InlineKeyboardButton> { 
        //    new InlineKeyboardButton("Назад", $"chosen_university,{uni.Id}"),
        //    new InlineKeyboardButton("Сховати", "hideMessage")
        //});

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

        //Department dep = await _apiClient.GetAsync<Department>($"/departments/{groupId}", userId, new Dictionary<string, string>());
        //buttonList.Add(new List<InlineKeyboardButton> { 
        //    new InlineKeyboardButton("Назад", $"chosen_department,{dep.Id}"),
        //    new InlineKeyboardButton("Сховати", "hideMessage")
        //});

        
        await _botClient.SendMessage(userId, $"Ви обрали групу {group.Name}", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowSchedule(long userId, DateOnly date)
    {
        DateTime dateTime = DateTime.Now;
        int a = date.DayNumber - DateOnly.FromDateTime(dateTime).DayNumber;
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

        buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("Сховати розклад", "hideMessage")});
        await _botClient.SendMessage(userId, $"Розклад на ", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowAssignmentInfo(long userId, int assignmentId)
    {

        //InlineKeyboardButton buttonNavi = new InlineKeyboardButton("Назад", $"show,{}");
        string assignmentInfo = "";

        Assignment assignment = await _apiClient.GetAsync<Assignment>($"/assignments/{assignmentId}", userId);
        assignmentInfo += assignment.StartTime.ToString() + " " + assignment.EndTime.ToString() + "\n\n";
        //assignmentInfo = 
        assignmentInfo += assignment.Lecturer.ToString();

        //await _botClient.SendMessage(userId, $"{assignmentInfo}", replyMarkup: new InlineKeyboardButton(buttonNavi));
    }
}

