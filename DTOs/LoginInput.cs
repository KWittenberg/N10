namespace N10.DTOs;

public sealed class LoginInput
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    public PasskeyInputModel? Passkey { get; set; }
}