namespace N10.DTOs;

public class AvatarInput
{
    public IBrowserFile? File { get; set; }
    //public IFormFile? File { get; set; }
}

public class AvatarInputValidator : AbstractValidator<AvatarInput>
{
    public AvatarInputValidator()
    {
        RuleFor(file => file.File).NotNull().WithMessage("File cannot be null!");

        RuleFor(file => file.File).NotEmpty().WithMessage("File cannot be empty!");

        RuleFor(file => file.File.ContentType).Must(BeAValidImage).WithMessage("Only .JPG and .PNG files are allowed!");

        // RuleFor(file => file.File.Length).LessThanOrEqualTo(2 * 1024 * 1024).WithMessage("File size cannot exceed 2 MB."); // 2 MB limit
    }

    bool BeAValidImage(string contentType) => contentType == "image/jpeg" || contentType == "image/png";
}