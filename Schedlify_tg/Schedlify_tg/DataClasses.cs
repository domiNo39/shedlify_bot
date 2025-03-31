using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

    // Navigation properties

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