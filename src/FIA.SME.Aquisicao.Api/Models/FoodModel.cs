using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FluentValidation.Results;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class FoodResponse
    {
        public FoodResponse(Food? food)
        {
            if (food == null)
                return;

            this.id = food.id;
            this.name = food.name;
            this.category_id = food.category_id;
            this.measure_unit = food.measure_unit;
            this.is_active = food.is_active;
        }

        public Guid id          { get; set; }
        public Guid category_id { get; set; }
        public string name      { get; set; }
        public int measure_unit { get; set; }
        public bool is_active   { get; set; }

        public string measure_unit_name { get { return ((MeasureUnitEnum)this.measure_unit).DescriptionAttr(); } }
    }

    public class FoodRegister : BaseDomain
    {
        public Guid category_id { get; set; }
        public string name      { get; set; }
        public int measure_unit { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new FoodRegisterValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class FoodUpdate : BaseDomain
    {
        public Guid id          { get; set; }
        public Guid category_id { get; set; }
        public string name      { get; set; }
        public int measure_unit { get; set; }
        public bool is_active   { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new FoodUpdateValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
