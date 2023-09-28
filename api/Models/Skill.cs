using System.ComponentModel.DataAnnotations.Schema;

namespace westcoast_education.api.Data.Models;

public class Skill
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int? TeacherId { get; set; }

    [ForeignKey("TeacherId")]
    public Teacher? Teacher { get; set; }
}