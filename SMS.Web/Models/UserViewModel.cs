using System.ComponentModel.DataAnnotations;
using SMS.Data.Entities;

namespace SMS.Web.Models;
    
public class UserViewModel
{       
    // TBC add attributes
    [Required]
    public string Name {get; set;}
    [Required]
    [EmailAddress]
    public string Email{get; set;}
    [Required]
    [StringLength (12, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
    public string Password { get; set; }
    [Compare(nameof(Password), ErrorMessage ="Passwords don't match")]
    [Display (Name="Confirm Password")]
    public string PasswordConfirm { get; set;}
    [Required]
    public Role Role {get; set;}
}
