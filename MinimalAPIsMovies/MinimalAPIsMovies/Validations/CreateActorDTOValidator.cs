using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(ValidationUtilities.NoEmptyMessage)
                .MaximumLength(150).WithMessage(ValidationUtilities.MaximumLengthMessage);

            var minimalDate = new DateTime(1900, 1, 1);

            RuleFor(p => p.DateOfBirth).GreaterThanOrEqualTo(minimalDate)
                .WithMessage(ValidationUtilities.GreaterThanDate(minimalDate));
        }
    }
}
