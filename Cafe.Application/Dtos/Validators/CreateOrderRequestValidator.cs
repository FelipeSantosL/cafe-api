using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos.Validators
{
    public class CreateOrderRequestValidator: AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator() 
        {
            RuleFor(x => x.Items).NotEmpty().WithMessage("Pedido precisa de pelo menos um item");
            RuleForEach(x => x.Items).ChildRules(Items =>
            {
                Items.RuleFor(i => i.ProductId).NotEmpty();
                Items.RuleFor(i => i.Quantity).GreaterThan(0);

            });
        }
    }
}
