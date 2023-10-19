using FluentValidation;
using FluentValidation.Validators;

namespace FIA.SME.Aquisicao.Core.Validations
{
    public class DateTimeValidator<T, TProp> : PropertyValidator<T, TProp>
    {
        private readonly bool _notNull;

        public DateTimeValidator(bool notNull) : base()
        {
            this._notNull = notNull;
        }

        public override string Name => "DateTimeValidator";

        public override bool IsValid(ValidationContext<T> context, TProp value)
        {
            if (value == null && !this._notNull) return true;

            if (value?.ToString() == null) return false;

            DateTime buffer;
            return DateTime.TryParse(value.ToString(), out buffer);
        }
    }

    public static class StaticDateTimeValidator
    {
        public static IRuleBuilderOptions<T, TProperty> IsValidDateTime<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool notNull)
        {
            return ruleBuilder.SetValidator(new DateTimeValidator<T, TProperty>(notNull));
        }
    }
}
