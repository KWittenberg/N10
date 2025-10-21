namespace N10.DTOs;

public class EmailInput
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Subject min 3 chars!")]
    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}