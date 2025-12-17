namespace N10.DTOs;

public class UserInput
{
    public int? Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? CompanyName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; } = string.Empty;


    public string? Country { get; set; } = string.Empty;

    public string? Zip { get; set; } = string.Empty;

    public string? City { get; set; } = string.Empty;

    public string? Street { get; set; } = string.Empty;
}


public class UserInputValidator : AbstractValidator<UserInput>
{
    public UserInputValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(ApplicationUserConst.FirstNameMinLength).WithMessage("{PropertyName} too short!")
            .MaximumLength(ApplicationUserConst.FirstNameLength).WithMessage("{PropertyName} too long!");

        RuleFor(x => x.LastName)
            .MinimumLength(ApplicationUserConst.LastNameMinLength).WithMessage("{PropertyName} too short!")
            .MaximumLength(ApplicationUserConst.LastNameLength).WithMessage("{PropertyName} too long!");
    }
}