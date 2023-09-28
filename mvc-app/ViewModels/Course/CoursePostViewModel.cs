using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvc_app.ViewModels.Course;

public class CoursePostViewModel
{
    [Required(ErrorMessage = "Titel krävs")]
    [DisplayName("Titel")]
    public string? Title { get; set; }
    [Required(ErrorMessage = "Kursnummer krävs")]
    [DisplayName("Kursnummer")]
    public int CourseNumber { get; set; }
    [Required(ErrorMessage = "Längd krävs")]
    [DisplayName("Längd (antal veckor)")]
    public int WeeksDuration { get; set; }
    [DisplayName("Lärare")]
    public string? TeacherId { get; set; }
    public List<SelectListItem>? Teachers { get; set; }
    [Required(ErrorMessage = "Startdatum krävs")]
    [DisplayName("Startdatum")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly EndDate { get; set; }

}