using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Setup.Models.Account;

[BindProperties]
public class AccountModel
{
    [Key] public long Id { get; set; }

    [Required]
    [StringLength(12, MinimumLength = 4, ErrorMessage = "You username must be between 4-12 characters.")]
    public string? Username { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email-address")]
    public string? Email { get; set; }

    [Required]
    [PasswordPropertyText]
    [StringLength(24, MinimumLength = 8, ErrorMessage = "Your password must be between 8-24 characters.")]
    public string? Password { get; set; }

    [Required]
    [DisplayName("Confirm password")]
    [NotMapped] // Does not affect database
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [PasswordPropertyText]
    public string? ConfirmPassword { get; set; }
}