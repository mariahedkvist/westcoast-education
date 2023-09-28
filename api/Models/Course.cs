using System.ComponentModel.DataAnnotations.Schema;
using westcoast_education.api.Models;

namespace westcoast_education.api.Data.Models;

public class Course
{
    public int CourseId { get; set; }
    public string? Title { get; set; }
    public int CourseNumber { get; set; }
    public int WeeksDuration { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CourseStatusEnum Status { get; set; }

    public int? TeacherId { get; set; }

    public ICollection<Student>? Students { get; set; }

    [ForeignKey("TeacherId")]
    public Teacher? Teacher { get; set; }
}