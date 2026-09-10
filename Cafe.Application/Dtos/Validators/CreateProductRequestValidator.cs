using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(150);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Categoria é obrigatória");
        }
    }
}