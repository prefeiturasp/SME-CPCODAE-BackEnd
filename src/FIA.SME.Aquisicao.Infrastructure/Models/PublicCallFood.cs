using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallFood
    {
        #region [ Construtores ]

        internal PublicCallFood()
        {
            this.creation_date = DateTime.UtcNow.SetKindUtc();
        }

        public PublicCallFood(Guid id, Guid food_id, Guid public_call_id, decimal price, MeasureUnitEnum measure_unit, decimal quantity, bool accepts_organic, bool is_organic, bool is_active)
        {
            this.id = id;
            this.food_id = food_id;
            this.public_call_id = public_call_id;
            this.price = price;
            this.quantity = quantity;
            this.measure_unit = measure_unit;
            this.creation_date = DateTime.UtcNow.SetKindUtc();
            this.accepts_organic = accepts_organic;
            this.is_organic = is_organic;
            this.is_active = is_active;
        }

        internal PublicCallFood(ChamadaPublicaAlimento chamadaPublicaAlimento)
        {
            this.id = chamadaPublicaAlimento.id;
            this.food_id = chamadaPublicaAlimento.alimento_id;
            this.public_call_id = chamadaPublicaAlimento.chamada_publica_id;
            this.price = chamadaPublicaAlimento.preco;
            this.quantity = chamadaPublicaAlimento.quantidade;
            this.creation_date = chamadaPublicaAlimento.data_criacao.ToLocalTime();
            this.accepts_organic = chamadaPublicaAlimento.aceita_organico;
            this.is_organic = chamadaPublicaAlimento.organico;
            this.is_active = chamadaPublicaAlimento.ativo;

            if (chamadaPublicaAlimento.Alimento != null)
            {
                this.food = new Food(chamadaPublicaAlimento.Alimento);
                this.measure_unit = (MeasureUnitEnum)chamadaPublicaAlimento.Alimento.unidade_medida;
            }
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                      { get; private set; }
        public Guid food_id                 { get; private set; }
        public Guid public_call_id          { get; private set; }
        public decimal price                { get; private set; }
        public decimal quantity             { get; private set; }
        public MeasureUnitEnum measure_unit { get; private set; }
        public DateTime creation_date       { get; private set; }
        public bool accepts_organic          { get; private set; }
        public bool is_organic              { get; private set; }
        public bool is_active               { get; private set; }

        public Food food                    { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void SetId(Guid id)
        {
            this.id = id;
        }

        public void SetPublicCallId(Guid id)
        {
            this.public_call_id = id;
        }

        #endregion [ FIM - Metodos ]
    }
}
