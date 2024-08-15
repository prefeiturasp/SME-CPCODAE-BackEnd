using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCall
    {
        #region [ Construtores ]

        public PublicCall(Guid id, int city_id, string number, string name, string type, string agency, string process, DateTime registration_start_date, DateTime registration_end_date, 
            DateTime public_session_date, string public_session_place, string public_session_url, string notice_url, string notice_object, string delivery_information, string? extra_information, 
            bool is_active)
        {
            this.id = id;
            this.city_id = city_id;
            this.number = number;
            this.name = name;
            this.type = type;
            this.agency = agency;
            this.process = process;
            this.registration_start_date = registration_start_date.ConvertToSolveUTC();
            this.registration_end_date = registration_end_date.ConvertToSolveUTC();
            this.public_session_date = public_session_date.ConvertToSolveUTC();
            this.public_session_place = public_session_place;
            this.public_session_url = public_session_url;
            this.notice_url = notice_url;
            this.notice_object = notice_object;
            this.delivery_information = delivery_information;
            this.extra_information = extra_information;
            this.is_active = is_active;

            this.status_id = (int)PublicCallStatusEnum.EmAndamento;
        }

        internal PublicCall(ChamadaPublica chamadaPublica)
        {
            if (chamadaPublica == null)
                return;

            this.id = chamadaPublica.id;
            this.city_id = chamadaPublica.codigo_cidade_ibge;
            this.data_cancelamento = chamadaPublica.data_cancelamento;
            this.data_deserta = chamadaPublica.data_deserta;
            this.data_contratacao = chamadaPublica.data_contratacao;
            this.data_contrato_executado = chamadaPublica.data_contrato_executado;
            this.data_habilitacao = chamadaPublica.data_habilitacao;
            this.data_homologacao = chamadaPublica.data_homologacao;
            this.data_suspensao = chamadaPublica.data_suspensao;
            this.number = chamadaPublica.numero;
            this.name = chamadaPublica.nome;
            this.type = chamadaPublica.tipo;
            this.agency = chamadaPublica.orgao;
            this.process = chamadaPublica.processo;
            this.creation_date = chamadaPublica.data_criacao;
            this.registration_start_date = chamadaPublica.data_inscricao_inicio;
            this.registration_end_date = chamadaPublica.data_inscricao_termino;
            this.public_session_date = chamadaPublica.data_sessao_publica;
            this.public_session_place = chamadaPublica.sessao_publica_local;
            this.public_session_url = chamadaPublica.sessao_publica_url;
            this.notice_url = chamadaPublica.edital_url;
            this.notice_object = chamadaPublica.objeto;
            this.delivery_information = chamadaPublica.estimativa;
            this.extra_information = chamadaPublica.informacoes_adicionais;
            this.status_id = chamadaPublica.status_id;
            this.is_active = chamadaPublica.ativa;

            if (chamadaPublica.ChamadaPublicaAlimentos != null && chamadaPublica.ChamadaPublicaAlimentos.Any())
            {
                this._foods = chamadaPublica.ChamadaPublicaAlimentos.Select(f => new PublicCallFood(f)).ToList();
            }

            if (chamadaPublica.ChamadaPublicaDocumentos != null && chamadaPublica.ChamadaPublicaDocumentos.Any())
            {
                this._documents = chamadaPublica.ChamadaPublicaDocumentos.Select(d => new PublicCallDocument(d)).ToList();
            }
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                          { get; private set; }
        public int city_id                      { get; private set; }
        public string delivery_information      { get; private set; }
        public string? extra_information        { get; private set; }
        public string notice_url                { get; private set; }
        public string name                      { get; private set; }
        public string number                    { get; private set; }
        public string notice_object             { get; private set; }
        public string agency                    { get; private set; }
        public string process                   { get; private set; }
        public string public_session_place      { get; private set; }
        public string public_session_url        { get; private set; }
        public string type                      { get; private set; }
        public int status_id                    { get; private set; }
        public DateTime creation_date           { get; private set; }
        public DateTime registration_start_date { get; private set; }
        public DateTime registration_end_date   { get; private set; }
        public DateTime public_session_date     { get; private set; }
        public DateTime? data_habilitacao       { get; private set; }
        public DateTime? data_homologacao       { get; private set; }
        public DateTime? data_contratacao       { get; private set; }
        public DateTime? data_contrato_executado { get; private set; }
        public DateTime? data_suspensao         { get; private set; }
        public DateTime? data_cancelamento      { get; private set; }
        public DateTime? data_deserta           { get; private set; }
        public bool is_active                   { get; private set; }

        //public int status
        //{
        //    get
        //    {
        //        if (!this.is_active)
        //            return (int)PublicCallStatusEnum.Cancelada;

        //        var doesNotHaveDeliveryInfo = (this._foods == null || this._foods.Count == 0);

        //        if (doesNotHaveDeliveryInfo)
        //            return this.already_been_chosen ? (int)PublicCallStatusEnum.AguardandoConfirmacao : (int)PublicCallStatusEnum.AguardandoCompra;

        //        return this.is_all_delivered ? (int)PublicCallStatusEnum.Finalizada : (int)PublicCallStatusEnum.AguardandoEntrega;
        //    }
        //}

        private List<PublicCallDocument> _documents { get; set; }
        public IReadOnlyCollection<PublicCallDocument> documents => this._documents;

        private List<PublicCallFood> _foods { get; set; }
        public IReadOnlyCollection<PublicCallFood> foods => this._foods;

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void AddFood(PublicCallFood food)
        {
            if (this._foods == null)
                this._foods = new List<PublicCallFood>();

            this._foods.Add(food);
        }

        public void AddDocument(PublicCallDocument document)
        {
            if (this._documents == null || this._documents.Count == 0)
                this._documents = new List<PublicCallDocument>();

            this._documents.Add(document);
        }

        public void Disable()
        {
            this.is_active = false;

            SetStatus(PublicCallStatusEnum.Cancelada);
        }

        public void SetAsDeserta()
        {
            this.is_active = false;

            SetStatus(PublicCallStatusEnum.Deserta);
        }

        public void Suspend()
        {
            this.is_active = false;

            SetStatus(PublicCallStatusEnum.Suspensa);
        }

        public static string GetColorClass(decimal total_delivered_percentage)
        {
            switch (total_delivered_percentage)
            {
                case var _ when total_delivered_percentage <= 10:
                    return "red";
                case var _ when (total_delivered_percentage > 10 && total_delivered_percentage <= 45):
                    return "orange";
                case var _ when total_delivered_percentage < 100:
                    return "yellow";
                default:
                    return "green";
            }
        }

        public void SetAsCompleted()
        {
            this.is_active = false;

            SetStatus(PublicCallStatusEnum.CronogramaExecutado);
        }

        public void SetId(Guid id)
        {
            this.id = id;
        }

        public void SetStatus(PublicCallStatusEnum status)
        {
            this.status_id = (int)status;

            switch (status)
            {
                case PublicCallStatusEnum.Aprovada:
                    this.data_habilitacao = DateTime.Now;
                    break;
                case PublicCallStatusEnum.Homologada:
                    this.data_homologacao = DateTime.Now;
                    break;
                case PublicCallStatusEnum.Contratada:
                    this.data_contratacao = DateTime.Now;
                    break;
                case PublicCallStatusEnum.CronogramaExecutado:
                    this.data_contrato_executado = DateTime.Now;
                    break;
                case PublicCallStatusEnum.Suspensa:
                    this.data_suspensao = DateTime.Now;
                    break;
                case PublicCallStatusEnum.Cancelada:
                    this.data_cancelamento = DateTime.Now;
                    break;
                case PublicCallStatusEnum.Deserta:
                    this.data_deserta = DateTime.Now;
                    break;
                default:
                    break;
            }
        }

        #endregion [ FIM - Metodos ]
    }
}
