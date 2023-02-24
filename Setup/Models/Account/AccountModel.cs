using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Setup.Models.Account;

[BindProperties]
public class AccountModel
{
    [Key] public long Id { get; set; }

    [Required(ErrorMessage = "Required")]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Username must be 4-20 characters.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Required")]
    [EmailAddress(ErrorMessage = "Please use a valid email-address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Required")]
    [PasswordPropertyText]
    [StringLength(24, MinimumLength = 8, ErrorMessage = "Password must be 8-24 characters.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Required")]
    [DisplayName("Confirm password")]
    [NotMapped] // Does not affect database
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [PasswordPropertyText]
    public string? ConfirmPassword { get; set; }
}