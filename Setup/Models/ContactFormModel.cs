using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Setup.Models;

[BindProperties]
public class ContactFormModel
{
    [Key] public long Id { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email-address")]
    public string? Email { get; set; }

    [Required]
    [StringLength(200, ErrorMessage = "The input cannot be longer than 200 characters.")]
    public string? Subject { get; set; }

    [Required]
    [StringLength(600, ErrorMessage = "The input cannot be longer than 600 characters.")]
    public string? Message { get; set; }
}