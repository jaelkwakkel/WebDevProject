using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Setup.Models;

[BindProperties]
public class ContactFormModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required]
    [StringLength(200, ErrorMessage = "The input cannot be longer than 200 characters.")]
    public string Description { get; set; }

    [Required]
    [StringLength(600, ErrorMessage = "The input cannot be longer than 600 characters.")]
    public string Message { get; set; }
}