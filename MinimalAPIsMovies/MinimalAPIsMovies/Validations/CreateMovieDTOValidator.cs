using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations
{
    public class CreateMovieDTOValidator : AbstractValidator<CreateMovieDTO>
    {
        public CreateMovieDTOValidator()
        {
            RuleFor(x=>x.Title)
                .NotEmpty().WithMessage(ValidationUtilities.NoEmptyMessage)
                .MaximumLength(250).WithMessage(ValidationUtilities.MaximumLengthMessage);
        }
    }
}
