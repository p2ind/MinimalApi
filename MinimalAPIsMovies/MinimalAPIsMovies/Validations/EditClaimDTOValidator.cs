﻿using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations
{
    public class EditClaimDTOValidator : AbstractValidator<EditClaimDTO>
    {
        public EditClaimDTOValidator()
        {
            RuleFor(x=>x.Email).NotEmpty().WithMessage(ValidationUtilities.NoEmptyMessage);
        }
    }
}
