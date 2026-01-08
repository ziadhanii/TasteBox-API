using TasteBox.Utilities;
using TasteBox.Utilities.File;

namespace TasteBox.Contracts.Category;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 40);

        RuleFor(x => x.ImageFile)
            .NotEmpty();

        RuleFor(x => x.ImageFile)
            .Must(FileValidationHelper.IsValidImageSize)
            .WithMessage(
                $"Image size exceeds the maximum allowed limit {FileSettings.MaxImageSizeInBytes / (1024 * 1024)} MB.");

        RuleFor(x => x.ImageFile)
            .Must(FileValidationHelper.IsValidImageExtension)
            .WithMessage(
                $"Image file extension is not allowed. Allowed extensions: {string.Join(", ", FileSettings.AllowedImageExtensions)}");
    }
}