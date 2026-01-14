using TasteBox.Utilities;
using TasteBox.Utilities.File;

namespace TasteBox.Contracts.Product;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.ImageFile)
            .Must(FileValidationHelper.IsValidImageSize)
            .WithMessage(
                $"Image size exceeds the maximum allowed limit {FileSettings.MaxImageSizeInBytes / (1024 * 1024)} MB.")
            .When(x => x.ImageFile != null);

        RuleFor(x => x.ImageFile)
            .Must(FileValidationHelper.IsValidImageExtension)
            .WithMessage(
                $"Image file extension is not allowed. Allowed extensions: {string.Join(", ", FileSettings.AllowedImageExtensions)}")
            .When(x => x.ImageFile != null);



        RuleFor(x => x.UnitId)
            .GreaterThan(0);
    }
}