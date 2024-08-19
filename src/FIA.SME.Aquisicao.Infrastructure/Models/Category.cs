using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class Category
    {
        #region [ Construtores ]

        internal Category(Categoria categoria)
        {
            if (categoria == null)
                return;

            this.id = categoria.id;
            this.name = categoria.nome;
            this.is_active = categoria.ativa;
        }

        public Category(Guid id, string name, bool is_active)
        {
            this.id = id;
            this.name = name;
            this.is_active = is_active;
        }

        public Category(string name) : this(Guid.NewGuid(), name, true) { }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id          { get; private set; }
        public string name      { get; private set; }
        public bool is_active   { get; private set; }

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
