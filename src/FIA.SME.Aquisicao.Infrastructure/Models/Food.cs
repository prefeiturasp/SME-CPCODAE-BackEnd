using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class Food
    {
        #region [ Construtores ]

        internal Food(Alimento alimento)
        {
            if (alimento == null)
                return;

            this.id = alimento.id;
            this.category_id = alimento.categoria_id;
            this.name = alimento.nome;
            this.measure_unit = alimento.unidade_medida;
            this.is_active = alimento.ativo;

            if (alimento.Categoria != null)
                this.category = new Category(alimento.Categoria);
        }

        public Food(Guid id, Guid category_id, string name, MeasureUnitEnum measure_unit, bool is_active)
        {
            this.id = id;
            this.category_id = category_id;
            this.name = name;
            this.measure_unit = (int)measure_unit;
            this.is_active = is_active;
        }

        public Food(Guid category_id, string name, MeasureUnitEnum measure_unit) : this(Guid.NewGuid(), category_id, name, measure_unit, true) { }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id              { get; private set; }
        public Guid category_id     { get; private set; }
        public string name          { get; private set; }
        public int measure_unit     { get; private set; }
        public bool is_active       { get; private set; }

        public Category category    { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void Disable()
        {
            this.is_active = false;
        }

        public void SetId(Guid id)
        {
            this.id = id;
        }

        #endregion [ FIM - Metodos ]
    }
}
