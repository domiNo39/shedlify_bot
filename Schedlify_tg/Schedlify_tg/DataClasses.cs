public class TgUserBase
{
    public string? Username { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
}
public class TgUser
{
    public long Id { get; set; }
    public string? Username { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? GroupId { get; set; }

    public bool Subscribed { get; set; }
}

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

public class Assignment
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int ClassId { get; set; }

    public ClassType? ClassType { get; set; }
    public Mode? Mode { get; set; }

    public Weekday Weekday { get; set; }

    public string? Lecturer { get; set; }
    public string? Address { get; set; }
    public string? RoomNumber { get; set; }

   
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

 
    public AssignmentType Type { get; set; }

    public DateOnly? Date { get; set; }

}

public class Class
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string Name { get; set; }
}
    public enum ClassType
{
    Lecture,
    Seminar
}

public enum Mode
{
    Online,
    Offline
}

public enum Weekday
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}

public enum AssignmentType
{
    Special,
    Regular,
    Even,
    Odd
}

public class Dicts
{
    public static readonly Dictionary<Mode, string> Modes = new Dictionary<Mode, string>()
    {
        {Mode.Online, "Дистанційно"},
        {Mode.Offline, "Очно" }
    };
    public static readonly Dictionary<Weekday, string> WeekDays = new Dictionary<Weekday, string>()
    {
        {Weekday.Monday, "Понеділок" },
        {Weekday.Tuesday, "Вівторок" },
        {Weekday.Wednesday,"Середа" },
        {Weekday.Thursday, "Четвер" },
        {Weekday.Friday, "П'ятниця" },
        {Weekday.Saturday, "Субота" },
        {Weekday.Sunday, "Неділя" }
    };
    public static readonly Dictionary<ClassType, string> ClassTypes = new Dictionary<ClassType, string>()
    {
        {ClassType.Lecture, "Лекція" },
        {ClassType.Seminar, "Практична" }
    };
    public static readonly Dictionary<AssignmentType, string> AssignmentTypes = new Dictionary<AssignmentType, string>()
    {
        {AssignmentType.Special, "Одноразова"},
        {AssignmentType.Regular, "Регулярна"},
        {AssignmentType.Odd, "Періодична"},
        {AssignmentType.Even, "Періодична"}
    };
    public static readonly Dictionary<int, string> Months = new Dictionary<int, string>()
    {
        {1, "січня"},
        {2, "лютого"},
        {3, "березня"},
        {4, "квітня"},
        {5, "травня"},
        {6, "червня"},
        {7, "липня"},
        {8, "серпня"},
        {9, "вересня"},
        {10, "жовтня"},
        {11, "листопада"},
        {12, "грудня"}
    };
}
