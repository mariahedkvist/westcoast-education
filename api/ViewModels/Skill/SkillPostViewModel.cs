using System.ComponentModel.DataAnnotations;

namespace westcoast_education.api.ViewModels;

public class SkillPostViewModel
{
    [Required(ErrorMessage = "Namn krävs")]
    public string? Name { get; set; }
}