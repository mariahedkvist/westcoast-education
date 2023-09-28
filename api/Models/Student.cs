using System.ComponentModel.DataAnnotations.Schema;

namespace westcoast_education.api.Data.Models;

public class Student : Person
{
    public int? CourseId { get; set; }
    
    [ForeignKey("CourseId")]
    public Course? Course { get; set; }
}