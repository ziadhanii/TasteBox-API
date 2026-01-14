using TasteBox.Utilities;
using TasteBox.Utilities.File;

namespace TasteBox.Contracts.Product;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

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

        RuleFor(x => x.CostPrice)
            .GreaterThan(0);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);

        RuleFor(x => x.DiscountedPrice)
            .GreaterThan(0)
            .LessThan(x => x.UnitPrice)
            .GreaterThan(x => x.CostPrice)
            .When(x => x.DiscountedPrice.HasValue);

        RuleFor(x => x.MinOrderQty)
            .GreaterThan(0)
            .LessThan(x => x.MaxOrderQty);

        RuleFor(x => x.MaxOrderQty)
            .GreaterThan(x => x.MinOrderQty);

        RuleFor(x => x.UnitId)
            .NotEmpty()
            .GreaterThan(0);
    }
}