using System.ComponentModel.DataAnnotations;

namespace westcoast_education.api.ViewModels;

public class CourseUpdateViewModel
{
    [Required(ErrorMessage = "Titel krävs")]
    public string? Title { get; set; }
    [Required(ErrorMessage = "Kursnummer krävs")]
    public int CourseNumber { get; set; }
    [Required(ErrorMessage = "Längd krävs")]
    public int WeeksDuration { get; set; }
    [Required(ErrorMessage = "Startdatum krävs")]
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}