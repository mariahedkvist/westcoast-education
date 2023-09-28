namespace mvc_app.ViewModels.Course;

public class CourseListViewModel
{
    public int CourseId { get; set; }
    public string? Title { get; set; }
    public int CourseNumber { get; set; }
    public string? Teacher { get; set; }
    public int WeeksDuration { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}