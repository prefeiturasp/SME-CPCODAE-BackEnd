using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class Configuration
    {
        #region [ Construtores ]

        internal Configuration(Configuracao configuracao)
        {
            if (configuracao == null)
                return;

            this.id = configuracao.id;
            this.name = configuracao.nome;
            this.value = configuracao.valor;
            this.is_active = configuracao.ativa;
        }

        public Configuration(Guid id, string name, string valor, bool is_active)
        {
            this.id = id;
            this.name = name;
            this.value = valor;
            this.is_active = is_active;
        }

        public Configuration(string name, string valor) : this(Guid.NewGuid(), name, valor, true) { }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id          { get; private set; }
        public string name      { get; private set; } = String.Empty;
        public string value     { get; private set; } = String.Empty;
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
