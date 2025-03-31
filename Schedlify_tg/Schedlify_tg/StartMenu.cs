using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

public class StartMenu
{
    private readonly ITelegramBotClient _botClient;
    private List<University> _universities;
    private List<Department> _departments;
    private List<Group> _groups;
    private ApiClient _apiClient;

    public StartMenu(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _apiClient = new ApiClient();
        _departments = new List<Department>();
        _groups = new List<Group>();
    }

    public async void ShowUniversityChooseList(long userId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        _universities = await _apiClient.GetAsync<List<University>>("/universities", userId);
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
        _departments = await _apiClient.GetAsync<List<Department>>("/departments", userId, _params);
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
        _groups = await _apiClient.GetAsync<List<Group>>("/groups", userId, _params);
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
        buttonList.Add(new List<InlineKeyboardButton> { 
            new InlineKeyboardButton("Підписатись", "subscribe"), 
            new InlineKeyboardButton("Переглянути розклад", "show")});

        await _botClient.SendMessage(userId, $"Ви обрали групу {group.Name}", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }

    public async void ShowSchedule(long userId)
    {

    }
}
