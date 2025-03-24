using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class University
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UniversityId { get; set; }
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DepartmentId { get; set; }
}

public class StartMenu
{
    private readonly ITelegramBotClient _botClient;
    private readonly List<University> _universities;
    private readonly List<Department> _departments;
    private readonly List<Group> _groups;

    public StartMenu(ITelegramBotClient botClient)
    {
        _botClient = botClient;

        _universities = new List<University>
        {
            new University { Name = "ЛНУ1", Id = 1 },
            new University { Name = "ЛНУ2", Id = 2 },
            new University { Name = "ЛНУ3", Id = 3 },
            new University { Name = "ЛНУ4", Id = 4 },
            new University { Name = "ЛНУ5", Id = 5 },
        };

        _departments = new List<Department>
        {
            new Department { Name = "ФПМІ11", Id = 1, UniversityId = 1 },
            new Department { Name = "ФПМІ22", Id = 2, UniversityId = 2 },
            new Department { Name = "ФПМІ33", Id = 3, UniversityId = 3 },
            new Department { Name = "ФПМІ44", Id = 4, UniversityId = 4 },
            new Department { Name = "ФПМІ51", Id = 5, UniversityId = 1 },
            new Department { Name = "ФПМІ62", Id = 6, UniversityId = 2 },
            new Department { Name = "ФПМІ73", Id = 7, UniversityId = 3 },
            new Department { Name = "ФПМІ84", Id = 8, UniversityId = 4 },
            new Department { Name = "ФПМІ95", Id = 9, UniversityId = 5 },
        };

        _groups = new List<Group>
        {
            new Group { Name = "ПМІ-11 11", Id = 1, DepartmentId = 1 },
            new Group { Name = "ПМІ-11 22", Id = 2, DepartmentId = 2 },
            new Group { Name = "ПМІ-11 31", Id = 3, DepartmentId = 1 },
            new Group { Name = "ПМІ-11 42", Id = 4, DepartmentId = 2 },
        };
    }

    public async void ShowUniversityChooseList(long userId)
    {
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        foreach (University uni in _universities)
        {
            buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton(uni.Name, $"chosen_university,{uni.Id}") });

        }

        await _botClient.SendMessage(userId, "Виберіть університет", replyMarkup: new InlineKeyboardMarkup(buttonList));
    }
}