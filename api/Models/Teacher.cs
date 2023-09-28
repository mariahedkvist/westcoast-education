namespace westcoast_education.api.Data.Models;

public class Teacher : Person
{
    public ICollection<Course>? Courses { get; set; }
    public ICollection<Skill>? Skills { get; set; }
}