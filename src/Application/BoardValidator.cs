using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application;

public class BoardValidator : AbstractValidator<Board>
{
    public BoardValidator()
    {
        RuleFor(x => x.BoardId).NotEmpty();
        RuleFor(x => x.Columns).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleForEach(x => x.Columns).SetValidator(new ColumnValidator());
    }
}

public class ColumnValidator : AbstractValidator<Column>
{
    public ColumnValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleForEach(x => x.Tickets).SetValidator(new TicketValidator());
    }
}

public class TicketValidator : AbstractValidator<Ticket>
{
    public TicketValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
    }
}

public static class BoardValidatorExtension
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}