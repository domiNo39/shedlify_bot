using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class StartMenu
{
    private readonly ITelegramBotClient _botClient;
    private ApiClient _apiClient;
    private readonly int defaultLimit = 8;

    private string returnButton_text = "↩️Назад";
    private string hideButton_text = "🔽Приховати";
    private string chooseUni_message = "📚 Вкажіть ваш університет, щоб отримати актуальний розклад.";
    private string chooseFaculty_message = "🎓 Обраний університет: {0}\n\n📚 Тепер оберіть свій факультет, щоб отримати список доступних груп";
    private string chooseDep_message = "🎓 Обраний університет: {0}\n🏛️ Обраний факультет: {1}\n\n👥 Тепер оберіть свою групу, щоб ми могли показувати точний розклад саме для вас.";
    private string scheduleGroup_message =
"🎓 Університет: {0}\n" +
"🏛️ Факультет: {1}\n" +
"👥 Група: {2}\n\n" +
"🌟 Чудово! Тепер усе налаштовано. Ось, що ти можеш робити далі:\n\n" +
"📌 /subscribe — підписатись(або відписатись) на розклад і отримувати сповіщення\n" +
"📅 /show_schedule — подивитись розклад групи, яку ти обрав/-ла\n\n";
    private string scheduleDay_message =
"📅 Дата: {0}\n" +
"🗓️ День тижня: {1}\n\n" +
"🔍 Натисни на предмет, щоб побачити детальну інформацію.\n\n" +
"🎓 Університет: {2}\n" +
"🏛️ Факультет: {3}\n" +
"👥 Група: {4}";

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
        buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(hideButton_text, "hideMessage") });

        await _botClient.SendMessage(userId, chooseUni_message, replyMarkup: new InlineKeyboardMarkup(buttonList));
    }
    public async void ShowDepartmentChooseList(long userId, int i, int universityId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        Dictionary<string, string> _params = new Dictionary<string, string>();
        _params.Add("universityId", $"{universityId}");
        _params.Add("offset", $"{defaultLimit * i}");
        _params.Add("limit", $"{defaultLimit}");
        University uni = await _apiClient.GetAsync<University>($"/universities/{universityId}", userId);
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
            new InlineKeyboardButton(returnButton_text, $"choose_university,0"),
            new InlineKeyboardButton(hideButton_text, "hideMessage")
        });

        await _botClient.SendMessage(userId, string.Format(chooseFaculty_message, uni.Name), replyMarkup: new InlineKeyboardMarkup(buttonList));
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
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(group.Name, $"chosen_group,{group.Id}") });

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
        Department dep = await _apiClient.GetAsync<Department>($"/departments/{departmentId}", userId);
        University uni = await _apiClient.GetAsync<University>($"/universities/{dep.UniversityId}", userId);
        buttonList.Add(new List<InlineKeyboardButton> {
            new InlineKeyboardButton(returnButton_text, $"chosen_university,0,{uni.Id}"),
            new InlineKeyboardButton(hideButton_text, "hideMessage")
        });

        await _botClient.SendMessage(userId, string.Format(chooseDep_message, uni.Name, dep.Name), replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowGroupChosen(long userId, int groupId)
    {


        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();

        Group group = await _apiClient.GetAsync<Group>($"/groups/{groupId}", userId);
        TgUser user = await _apiClient.PostAsync<TgUser>($"/tgusers/groups/{groupId}", userId, new Dictionary<string, string>());
        buttonList.Add(new List<InlineKeyboardButton> {
            new InlineKeyboardButton("Підписатись", "subscribe"),
            new InlineKeyboardButton("Переглянути розклад", "show,0")});

        Department dep = await _apiClient.GetAsync<Department>($"/departments/{group.DepartmentId}", userId, new Dictionary<string, string>());
        University uni = await _apiClient.GetAsync<University>($"/universities/{dep.UniversityId}", userId);
        buttonList.Add(new List<InlineKeyboardButton> {
            new InlineKeyboardButton(returnButton_text, $"chosen_department,0,{dep.Id}"),
            new InlineKeyboardButton(hideButton_text, "hideMessage")
        });


        await _botClient.SendMessage(userId, string.Format(scheduleGroup_message, uni.Name, dep.Name, group.Name), replyMarkup: new InlineKeyboardMarkup(buttonList));
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

        Dictionary<string, string> _params = new Dictionary<string, string>();
        _params.Add("groupId", $"{user.GroupId}");
        _params.Add("date", $"{date.ToString("yyyy-MM-dd")}");
        List<Assignment> assignmentList = await _apiClient.GetAsync<List<Assignment>>($"/assignments/by_group_id_and_date", userId, _params);
        
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();

        Group group = await _apiClient.GetAsync<Group>($"/groups/{user.GroupId}", userId);
        Department dep = await _apiClient.GetAsync<Department>($"/departments/{group.DepartmentId}", userId);
        University uni = await _apiClient.GetAsync<University>($"/universities/{dep.UniversityId}", userId);
        string dateString = $"{date.Day} {Dicts.Months[date.Month]}";
        Weekday kyivWeekday = (Weekday)(((int)date.DayOfWeek + 6) % 7);
        string dayWeek = Dicts.WeekDays[kyivWeekday];

        foreach (Assignment assignment in assignmentList)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton($"{assignment.StartTime}: {(await _apiClient.GetAsync<Class>($"/classes/{assignment.ClassId}", userId)).Name}", $"showAssignmentInfo,{a},{assignment.Id}") });

        }

        int b = 1;
        int c = 1;
        if (date.DayOfWeek == DayOfWeek.Monday)
        {
            b = 2;
        }
        if(date.DayOfWeek == DayOfWeek.Saturday)
        {
            c = 2;
        }

        buttonList.Add(new List<InlineKeyboardButton>
        {

            new InlineKeyboardButton("<-", $"show,{a-b}"),
            new InlineKeyboardButton("->", $"show,{a+c}")
        });

        buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton($"{hideButton_text} розклад", "hideMessage")});
        await _botClient.SendMessage(userId, string.Format(scheduleDay_message, dateString, dayWeek, uni.Name, dep.Name, group.Name), replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowAssignmentInfo(long userId, int i, int assignmentId)
    {

       
        List<InlineKeyboardButton> buttonList = new List<InlineKeyboardButton> { new InlineKeyboardButton(returnButton_text, $"show,{i}") };
      
        string assignmentInfo = "";

        Assignment assignment = await _apiClient.GetAsync<Assignment>($"/assignments/{assignmentId}", userId);
        Class _class = await _apiClient.GetAsync<Class>($"/classes/{assignment.ClassId}",userId);
        assignmentInfo += $"🕒 Час початку: {assignment.StartTime.ToShortTimeString()}\n";
        assignmentInfo += $"<b>📚 Предмет:</b> {_class.Name}\n\n";
        if (assignment.Mode is Mode mode)
        {
            assignmentInfo += $"<b>🌐 Формат:</b> {Dicts.Modes[mode]}\n";
        }
        if (assignment.RoomNumber is string room)
        {
            assignmentInfo += "<b>🏛️ Місце проведення:</b>\n";
            assignmentInfo += $"    <b>🏫 Аудиторія:</b> {room}\n";
        }
        if (assignment.Address is string address)
        {
            assignmentInfo += $"    <b>📍Адреса:</b> <i>{address}</i>\n";
        }
        if (assignment.ClassType is ClassType classType)
        {
            assignmentInfo += $"<b>🎓 Тип заняття:</b> {Dicts.ClassTypes[classType]}\n";
        }
        if (assignment.Type is AssignmentType assignmentType)
        {
            assignmentInfo += $"<b>📅 Тип пари:</b> {Dicts.AssignmentTypes[assignmentType]}\n";
        }
        if (assignment.Lecturer is string lecturer)
        {
            assignmentInfo += $"<b>👤 Викладач:</b> {lecturer}\n";
        }

        await _botClient.SendMessage(userId, assignmentInfo, replyMarkup: new InlineKeyboardMarkup(buttonList), parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
    }
}

