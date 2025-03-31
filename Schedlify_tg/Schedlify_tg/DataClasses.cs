using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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