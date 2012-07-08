using FluentValidation;

namespace Fluent
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.FamilyName).NotEmpty().Length(1, 15);
            RuleFor(x => x.GivenNames).NotEmpty().Length(1, 15);
        }
    }
}