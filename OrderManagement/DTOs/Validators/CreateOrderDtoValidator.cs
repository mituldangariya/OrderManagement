using FluentValidation;
using OrderManagement.Application.DTOs;
using OrderManagement.DTOs;

namespace OrderManagement.DTOs.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required.")
                .Matches("^[a-zA-Z ]+$").WithMessage("Customer name must contain only letters and spaces.")
                .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one order item is required.");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ItemName)
                    .NotEmpty().WithMessage("Item name is required.");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Item quantity must be greater than zero.");

                items.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0).WithMessage("Item unit price must be greater than zero.");
            });
        }
    }
}
