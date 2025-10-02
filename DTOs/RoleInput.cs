namespace N10.DTOs;

public class RoleInput
{
    public string Name { get; set; } = string.Empty;
}


public class RoleInputValidator : AbstractValidator<RoleInput>
{
    public RoleInputValidator(RoleManager<IdentityRole<Guid>> roleManager)
    {
        RuleFor(x => x.Name)
            //.Cascade(CascadeMode.Stop)  // Opcionalno: Zaustavi validaciju na prvoj grešci
            .NotEmpty().WithMessage("Name is required!")
            .MinimumLength(RoleConsts.NameMinLength).WithMessage("Name too short!")
            .MaximumLength(RoleConsts.NameLength).WithMessage("Name too long!")
            .MustAsync(async (name, cancellation) => !await roleManager.RoleExistsAsync(name)) // Asinhrona provjera odmah u validatoru
            .WithMessage("Role name already exists!");
    }
}