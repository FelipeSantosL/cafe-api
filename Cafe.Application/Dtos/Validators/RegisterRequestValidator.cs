using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos.Validators
{
    public class RegisterRequestValidator: AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
                .Matches("[A-Z]").WithMessage("Senha deve conter ao menos uma letra maiúscula")
                .Matches("[0-9]").WithMessage("Senha deve conter ao menos um número");
        }
    }
}
