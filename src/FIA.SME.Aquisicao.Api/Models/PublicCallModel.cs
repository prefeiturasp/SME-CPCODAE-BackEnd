using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    #region [ Responses ]

    public class PublicCallDetailResponse
    {
        public PublicCallDetailResponse(PublicCall? publicCall)
        {
            if (publicCall == null)
                return;

            this.id = publicCall.id;
            this.city_id = publicCall.city_id;
            this.number = publicCall.number;
            this.name = publicCall.name;
            this.type = publicCall.type;
            this.agency = publicCall.agency;
            this.process = publicCall.process;
            this.registration_start_date = publicCall.registration_start_date;
            this.registration_end_date = publicCall.registration_end_date;
            this.public_session_date = publicCall.public_session_date.ToLocalTime();
            this.public_session_url = publicCall.public_session_url;
            this.public_session_place = publicCall.public_session_place;
            this.notice_url = publicCall.notice_url;
            this.notice_object = publicCall.notice_object;
            this.delivery_information = publicCall.delivery_information;
            this.extra_information = publicCall.extra_information;
            this.status = publicCall.status_id;
            this.is_active = publicCall.is_active;

            if (publicCall.documents != null && publicCall.documents.Any())
            {
                this.documents = publicCall.documents.Select(d => new PublicCallDocumentResponse(d)).ToList();
            }

            if (publicCall.foods != null && publicCall.foods.Any())
            {
                this.foods = publicCall.foods.Select(f => new PublicCallFoodResponse(f)).ToList();
            }
        }

        public Guid id                          { get; set; }
        public int city_id                      { get; set; }
        public string number                    { get; set; }
        public string name                      { get; set; }
        public string type                      { get; set; }
        public string agency                    { get; set; }
        public string process                   { get; set; }
        public DateTime registration_start_date { get; set; }
        public DateTime registration_end_date   { get; set; }
        public DateTime public_session_date     { get; set; }
        public string public_session_url        { get; set; }
        public string public_session_place      { get; set; }
        public string notice_url                { get; set; }
        public string notice_object             { get; set; }
        public string delivery_information      { get; set; }
        public string? extra_information        { get; set; }
        public bool is_active                   { get; set; }

        public int status                       { get; set; }

        public List<PublicCallDocumentResponse> documents   { get; set; }
        public List<PublicCallFoodResponse> foods           { get; set; }
    }

    public class PublicCallListResponse
    {
        public PublicCallListResponse(PublicCall publicCall)
        {
            if (publicCall == null)
                return;

            this.id = publicCall.id;
            this.number = publicCall.number;
            this.name = publicCall.name;
            this.type = publicCall.type;
            this.registration_start_date = publicCall.registration_start_date.Date;
            this.registration_end_date = publicCall.registration_end_date.Date;
            this.public_session_date = publicCall.public_session_date.ToLocalTime();
            this.public_session_place = publicCall.public_session_place;
            this.public_session_url = publicCall.public_session_url;
            this.notice_object = publicCall.notice_object;
            this.delivery_information = publicCall.delivery_information;
            this.status = publicCall.status_id;
            this.is_active = publicCall.is_active;

            this.foods = new List<FoodInfo>();

            if (publicCall != null && publicCall.foods?.Count > 0)
            {
                var foods = publicCall.foods.OrderBy(f => f.creation_date).ToList();

                foreach (var food in foods)
                {
                    this.foods.Add(new FoodInfo()
                    {
                        food_name = food.food.name,
                        measure_unit = food.measure_unit.DescriptionAttr(),
                        quantity = food.quantity,
                        price = food.price
                    });
                }
            }
        }

        public Guid id                          { get; set; }
        public string number                    { get; set; }
        public string name                      { get; set; }
        public string type                      { get; set; }
        public string delivery_information      { get; set; }
        public string notice_object             { get; set; }
        public DateTime registration_start_date { get; set; }
        public DateTime registration_end_date   { get; set; }
        public DateTime public_session_date     { get; set; }
        public string public_session_place      { get; set; }
        public string public_session_url        { get; set; }
        public int status                       { get; set; }
        public bool is_active                   { get; set; }

        public List<FoodInfo> foods { get; set; }

        public class FoodInfo
        {
            public string food_name     { get; set; }
            public string measure_unit  { get; set; }
            public decimal quantity     { get; set; }
            public decimal price        { get; set; }
        }
    }

    public class PublicCallCooperativeListResponse
    {
        public PublicCallCooperativeListResponse(PublicCallAnswer publicCallAnswer)
        {
            if (publicCallAnswer == null || publicCallAnswer.public_call == null)
                return;

            this.id = publicCallAnswer.public_call.id;
            this.public_call_answer_id = publicCallAnswer.id;
            this.name = publicCallAnswer.public_call.name;
            this.process = publicCallAnswer.public_call.process;
            this.creation_date = publicCallAnswer.public_call.creation_date.ToLocalTime();
            this.public_session_date = publicCallAnswer.public_call.public_session_date.ToLocalTime();
            this.registration_end_date = publicCallAnswer.public_call.registration_end_date.ToLocalTime();
            this.total_proposal = publicCallAnswer.quantity_edited ?? publicCallAnswer.quantity;

            var food = publicCallAnswer.food;

            if (food is not null)
            {
                this.food_id = food.id;
                this.food_name = food.name;
                this.measure_unit = ((MeasureUnitEnum)food.measure_unit).DescriptionAttr();
            }

            this.status = publicCallAnswer.public_call.status_id;
            this.was_chosen = publicCallAnswer.was_chosen;

            if (publicCallAnswer.deliveries != null && publicCallAnswer.deliveries.Any())
            {
                this.delivery_info = new CooperativeDeliveryInfoResponse();
                this.delivery_info.delivery_progress = publicCallAnswer.deliveries
                                                        .Select(d => new PublicCallCooperativeInfoResponse.CooperativeDeliveryInfoProgressResponse(
                                                            d.id,
                                                            d.delivery_date.ToLocalTime(),
                                                            0,
                                                            d.delivery_quantity,
                                                            d.was_delivered,
                                                            d.delivered_confirmation_date?.ToLocalTime(),
                                                            d.delivered_quantity
                                                        )).ToList();

                decimal currentQuantity = 0;

                foreach (var delivery in this.delivery_info.delivery_progress)
                {
                    currentQuantity += delivery.delivery_quantity;
                    delivery.delivery_percentage = this.total_proposal <= 0 ? 0 : Math.Round((decimal)currentQuantity * 100 / this.total_proposal, 2);
                }
            }
        }

        public Guid id                      { get; set; }
        public Guid public_call_answer_id   { get; set; }
        public string name                  { get; set; }
        public string process               { get; set; }
        public DateTime creation_date       { get; set; }
        public DateTime public_session_date { get; set; }
        public DateTime registration_end_date { get; set; }
        public decimal total_proposal       { get; set; }
        public Guid food_id                 { get; set; }
        public string food_name             { get; set; }
        public string measure_unit          { get; set; }
        public int status                   { get; set; }
        public bool was_chosen              { get; set; }

        public CooperativeDeliveryInfoResponse delivery_info { get; set; }
    }

    public class CooperativeDeliveryInfoResponse
    {
        public string color_class                   { get { return PublicCall.GetColorClass(this.total_delivered_percentage); } }
        
        public decimal total_delivered_percentage
        {
            get
            {
                if (this.delivery_progress == null)
                    return 0;

                var delivery_progress = this.delivery_progress.Where(dp => dp.was_delivered);

                if (delivery_progress == null || !delivery_progress.Any())
                    return 0;

                return delivery_progress.Max(dp => dp.delivery_percentage);
            }
        }

        public List<PublicCallCooperativeInfoResponse.CooperativeDeliveryInfoProgressResponse> delivery_progress { get; set; } = new List<PublicCallCooperativeInfoResponse.CooperativeDeliveryInfoProgressResponse>();
    }

    #endregion [ FIM - Responses ]

    #region [ Requests ]

    public interface IPublicCallRequestModel
    {
        int city_id                         { get; set; }
        string number                       { get; set; }
        string name                         { get; set; }
        string process                      { get; set; }
        DateTime registration_start_date    { get; set; }
        DateTime registration_end_date      { get; set; }
        DateTime public_session_date        { get; set; }
        string public_session_url           { get; set; }
        string public_session_place         { get; set; }
        string notice_url                   { get; set; }
        string notice_object                { get; set; }
        string delivery_information         { get; set; }
    }

    public class PublicCallRegistrationRequest : IPublicCallRequestModel
    {
        public int city_id                      { get; set; }
        public string number                    { get; set; }
        public string name                      { get; set; }
        public string process                   { get; set; }
        public DateTime registration_start_date { get; set; }
        public DateTime registration_end_date   { get; set; }
        public DateTime public_session_date     { get; set; }
        public string public_session_url        { get; set; }
        public string public_session_place      { get; set; }
        public string notice_url                { get; set; }
        public string notice_object             { get; set; }
        public string delivery_information      { get; set; }
        public string? extra_information        { get; set; }

        public List<PublicCallDocumentRequest> documents { get; set; }
        public List<PublicCallFoodRegistrationRequest> foods { get; set; }
    }

    public class PublicCallUpdateRequest : IPublicCallRequestModel
    {
        public Guid id                          { get; set; }
        public int city_id                      { get; set; }
        public string number                    { get; set; }
        public string name                      { get; set; }
        public string process                   { get; set; }
        public DateTime registration_start_date { get; set; }
        public DateTime registration_end_date   { get; set; }
        public DateTime public_session_date     { get; set; }
        public string public_session_url        { get; set; }
        public string public_session_place      { get; set; }
        public string notice_url                { get; set; }
        public string notice_object             { get; set; }
        public string delivery_information      { get; set; }
        public string? extra_information        { get; set; }
        public bool is_active                   { get; set; }

        public List<PublicCallDocumentRequest> documents { get; set; }
        public List<PublicCallFoodUpdateRequest> foods { get; set; }
    }

    public class PublicCallValidateMembersRequest
    {
        public string file_base_64 { get; set; }
    }

    public class PublicCallDocumentRequest
    {
        public Guid? id { get; set; }
        public Guid? food_id { get; set; }
        public Guid document_type_id { get; set; }
        public Guid? public_call_id { get; set; }
    }

    #endregion [ FIM - Requests ]
}
