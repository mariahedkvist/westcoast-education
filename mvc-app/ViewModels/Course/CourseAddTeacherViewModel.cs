using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvc_app.ViewModels.Course;
public class CourseAddTeacherViewModel
{
    public string? TeacherId { get; set; }
    public List<SelectListItem>? Teachers { get; set; }
}