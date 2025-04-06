using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

public class StartMenu
{
    private readonly ITelegramBotClient _botClient;
    private ApiClient _apiClient;
    private readonly int defaultLimit = 8;



    public StartMenu(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _apiClient = new ApiClient();

    }

    public async void ShowUniversityChooseList(long userId, int i)
    {

        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        Dictionary<string, string> _params = new Dictionary<string, string>();
        _params.Add("offset", $"{defaultLimit * i}");
        _params.Add("limit", $"{defaultLimit}");
        List<University> _universities = await _apiClient.GetAsync<List<University>>($"/universities", userId, _params);
        foreach (University uni in _universities)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(uni.Name, $"chosen_university,0,{uni.Id}") });

        }

        List<InlineKeyboardButton> buttonNavigation = new List<InlineKeyboardButton>();
        if (i > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("<-", $"choose_university,{i - 1}"));
        }
        if ((await _apiClient.GetAsync<List<University>>($"/universities?offset={defaultLimit * (i + 1)}&limit={defaultLimit}", userId)).Count > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("->", $"choose_university,{i + 1}"));
        }
        buttonList.Add(buttonNavigation);
        buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("Сховати", "hideMessage") });

        await _botClient.SendMessage(userId, "Виберіть університет", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }
    public async void ShowDepartmentChooseList(long userId, int i, int universityId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        Dictionary<string, string> _params = new Dictionary<string, string>();
        _params.Add("universityId", $"{universityId}");
        _params.Add("offset", $"{defaultLimit * i}");
        _params.Add("limit", $"{defaultLimit}");
        List<Department> _departments = await _apiClient.GetAsync<List<Department>>($"/departments", userId, _params);
        foreach (Department dep in _departments)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(dep.Name, $"chosen_department,0,{dep.Id}") });

        }
        List<InlineKeyboardButton> buttonNavigation = new List<InlineKeyboardButton>();
        if (i > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("<-", $"chosen_university,{i - 1},{universityId}"));
        }
        _params["offset"] = $"{defaultLimit * (i + 1)}";
        if ((await _apiClient.GetAsync<List<Department>>($"/departments", userId, _params)).Count > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("->", $"chosen_university,{i + 1},{universityId}"));
        }
        buttonList.Add(buttonNavigation);
        buttonList.Add(new List<InlineKeyboardButton> {
            new InlineKeyboardButton("Назад", $"choose_university,0"),
            new InlineKeyboardButton("Сховати", "hideMessage")
        });

        await _botClient.SendMessage(userId, "Виберіть факультет", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowGroupChooseList(long userId, int i, int departmentId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        Dictionary<string, string> _params = new Dictionary<string, string>();
        _params.Add("departmentId", $"{departmentId}");
        _params.Add("offset", $"{defaultLimit * i}");
        _params.Add("limit", $"{defaultLimit}");
        List<Group> _groups = await _apiClient.GetAsync<List<Group>>($"/groups", userId, _params);
        foreach (Group group in _groups)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(group.Name, $"chosen_group,0,{group.Id}") });

        }
        List<InlineKeyboardButton> buttonNavigation = new List<InlineKeyboardButton>();
        if (i > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("<-", $"chosen_department,{i - 1},{departmentId}"));
        }
        _params["offset"] = $"{defaultLimit * (i + 1)}";
        if ((await _apiClient.GetAsync<List<Group>>($"/groups", userId, _params)).Count > 0)
        {
            buttonNavigation.Add(new InlineKeyboardButton("->", $"chosen_department,{i + 1},{departmentId}"));
        }
        //University uni = await _apiClient.GetAsync<University>($"/universities/{departmentId}", userId);
        //buttonList.Add(new List<InlineKeyboardButton> { 
        //    new InlineKeyboardButton("Назад", $"chosen_university,0,{uni.Id}"),
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
        //    new InlineKeyboardButton("Назад", $"chosen_department,0,{dep.Id}"),
        //    new InlineKeyboardButton("Сховати", "hideMessage")
        //});


        await _botClient.SendMessage(userId, $"Ви обрали групу {group.Name}", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowSchedule(long userId, DateOnly date)
    {
        DateTime dateTime = DateTime.Now;
        int a = date.DayNumber - DateOnly.FromDateTime(dateTime).DayNumber;
        TgUser user = await _apiClient.GetAsync<TgUser>("/tgusers", userId);
        if (user == null || user.GroupId == null)
        {
            await _botClient.SendMessage(
                userId,
                "Щоб переглянути розклад оберіть вашу групу",
                replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton> { new InlineKeyboardButton("Обрати університет", "choose_university,0") })
            );
        }
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

    public async void ShowAssignmentInfo(long userId, int i, int assignmentId)
    {

        InlineKeyboardButton buttonNavi = new InlineKeyboardButton("Назад", $"show,{i}");
        string assignmentInfo = "";

        Assignment assignment = await _apiClient.GetAsync<Assignment>($"/assignments/{assignmentId}", userId);
        Class _class = await _apiClient.GetAsync<Class>($"/classes/{assignment.ClassId}",userId);
        assignmentInfo += $"<b><i>{_class.Name}</i></b>\n\n";
        if (assignment.RoomNumber is string room)
        {
            assignmentInfo += $"<b>Аудиторія:</b> {room}\n";
        }
        assignmentInfo += $"<b>Початок:</b> {assignment.StartTime.ToShortTimeString()}\n";
        if (assignment.Mode is Mode mode)
        {
            assignmentInfo += $"<b>Режим:</b> {Dicts.Modes[mode]}\n";
        }
        if (assignment.Lecturer is string lecturer)
        {
            assignmentInfo += $"<b>Викладач:</b> {lecturer}\n";
        }
        if (assignment.Address is string address)
        {
            assignmentInfo += $"<b>Адреса:</b> {address}\n";
        }
        assignmentInfo += $"\n{Dicts.AssignmentTypes[assignment.Type]}";
    }
}

