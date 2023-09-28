namespace westcoast_education.api.ViewModels;

public class TeacherListViewModel
{
    public int Id { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public ICollection<SkillListViewModel>? Skills { get; set; }
    public ICollection<CourseListViewModel>? Courses { get; set; }

}