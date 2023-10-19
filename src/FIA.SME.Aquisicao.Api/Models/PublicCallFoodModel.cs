using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class PublicCallFoodResponse
    {
        public PublicCallFoodResponse(PublicCallFood? publicaCallFood)
        {
            if (publicaCallFood == null)
                return;

            this.id = publicaCallFood.id;
            this.food_id = publicaCallFood.food_id;
            this.public_call_id = publicaCallFood.public_call_id;
            this.price = publicaCallFood.price;
            this.quantity = publicaCallFood.quantity;
            this.measure_unit = publicaCallFood.measure_unit.DescriptionAttr();
            this.creation_date = publicaCallFood.creation_date;
            this.accepts_organic = publicaCallFood.accepts_organic;
            this.is_organic = publicaCallFood.is_organic;
            this.is_active = publicaCallFood.is_active;

            if (publicaCallFood.food != null)
            {
                this.food_name = publicaCallFood.food.name;

                if (publicaCallFood.food.category != null)
                {
                    this.category_id = publicaCallFood.food.category_id;
                    this.category_name = publicaCallFood.food.category.name;
                }
            }
        }

        public Guid id              { get; set; }
        public Guid category_id     { get; set; }
        public Guid food_id         { get; set; }
        public Guid public_call_id  { get; set; }
        public decimal price        { get; set; }
        public decimal quantity     { get; set; }
        public string measure_unit  { get; set; }
        public DateTime creation_date { get; set; }
        public bool accepts_organic  { get; set; }
        public bool is_organic      { get; set; }
        public bool is_active       { get; set; }

        public string category_name { get; set; }
        public string food_name     { get; set; }
    }

    public interface IPublicCallRequest
    {
        Guid food_id        { get; set; }
        int measure_unit_id { get; set; }
        decimal price       { get; set; }
        decimal quantity    { get; set; }
        bool is_organic     { get; set; }
    }

    public class PublicCallFoodRegistrationRequest : IPublicCallRequest
    {
        public Guid food_id         { get; set; }
        public int measure_unit_id  { get; set; }
        public decimal price        { get; set; }
        public decimal quantity     { get; set; }
        public bool accepts_organic  { get; set; }
        public bool is_organic      { get; set; }
    }

    public class PublicCallFoodUpdateRequest : IPublicCallRequest
    {
        public Guid id              { get; set; }
        public Guid food_id         { get; set; }
        public Guid public_call_id  { get; set; }
        public int measure_unit_id  { get; set; }
        public decimal price        { get; set; }
        public decimal quantity     { get; set; }
        public bool accepts_organic  { get; set; }
        public bool is_organic      { get; set; }
    }
}
