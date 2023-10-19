namespace FIA.SME.Aquisicao.Api.Models
{
    public class PublicCallConfirmDeliveryInfoRequest
    {
        public DateTime delivered_date      { get; set; }
        public decimal delivered_quantity   { get; set; }
    }
}
