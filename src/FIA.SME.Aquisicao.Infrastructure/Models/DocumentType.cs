using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class DocumentType
    {
        #region [ Construtores ]

        internal DocumentType(TipoDocumento documento)
        {
            if (documento == null)
                return;

            this.id = documento.id;
            this.name = documento.nome;
            this.application = documento.aplicacao;
            this.is_visible = documento.visivel;
            this.is_active = documento.ativo;
        }

        public DocumentType(Guid id, string name, int application, bool is_visible, bool is_active)
        {
            this.id = id;
            this.name = name;
            this.application = application;
            this.is_visible = is_visible;
            this.is_active = is_active;
        }

        public DocumentType(string name, int application) : this(Guid.NewGuid(), name, application, true, true) { }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id          { get; private set; }
        public string name      { get; private set; }
        public int application  { get; private set; }
        public bool is_active   { get; private set; }
        public bool is_visible  { get; private set; }

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
