using System.ComponentModel.DataAnnotations;

namespace westcoast_education.api.ViewModels;

public class StudentUpdateViewModel
{
    [Required(ErrorMessage = "Födelsedatum krävs")]
    public DateOnly BirthDate { get; set; }
    [Required(ErrorMessage = "Förnamn krävs")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "Efternamn krävs")]
    public string? LastName { get; set; }
    [Required(ErrorMessage = "E-post krävs")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Telefonnummer krävs")]
    public string? Phone { get; set; }
    [Required(ErrorMessage = "Gatuadress krävs")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "Postnummer krävs")]
    public string? PostalCode { get; set; }
    [Required(ErrorMessage = "Stad krävs")]
    public string? City { get; set; }
    [Required(ErrorMessage = "Land krävs")]
    public string? Country { get; set; }
}