using Domain.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Validators.Commands
{
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {

            RuleFor(x => x.Subject).NotNull().NotEmpty();
        }
    }
}
