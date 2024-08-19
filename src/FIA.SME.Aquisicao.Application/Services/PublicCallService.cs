using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Integrations;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IPublicCallService
    {
        Task<Guid> Add(PublicCall publicCall);
        Task ChangeStatus(Guid public_call_id, PublicCallStatusEnum status);
        Task<PublicCall?> Get(Guid id, bool keepTrack = false);
        Task<List<PublicCall>> GetAll();
        Task<List<PublicCall>> GetAllDashboard();
        Task<List<PublicCall>> GetAllOnGoingByCooperative(Guid cooperativeId);
        Task<List<PublicCallReport>> GetAllReport(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess);
        Task<List<PublicCallReportCooperative>> GetAllReportCooperatives(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess);
        Task<bool> SetAsAwaitingDelivery(Guid id);
        Task<bool> SetAsBought(Guid id);
        Task Update(PublicCall publicCall);
        Task<bool> UpdateIfIsCompleted(Guid id);
    }

    internal class PublicCallService : IPublicCallService
    {
        #region [ Propriedades ]

        private readonly IPublicCallRepository _publicCallRepository;
        private readonly IPublicCallAnswerRepository _publicCallAnswerRepository;
        private readonly IPublicCallDeliveryRepository _publicCallDeliveryRepository;
        private readonly IPublicCallDocumentRepository _publicCallDocumentRepository;
        private readonly IPublicCallFoodRepository _publicCallFoodRepository;
        private readonly IIBGEIntegration _iBGEIntegration;
        private readonly ILocationRegionRepository _locationRegionRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallService(
            IPublicCallRepository publicCallRepository,
            IPublicCallAnswerRepository publicCallAnswerRepository,
            IPublicCallDeliveryRepository publicCallDeliveryRepository,
            IPublicCallDocumentRepository publicCallDocumentRepository,
            IPublicCallFoodRepository publicCallFoodRepository,
            IIBGEIntegration iBGEIntegration,
            ILocationRegionRepository locationRegionRepository)
        {
            this._publicCallRepository = publicCallRepository;
            this._publicCallAnswerRepository = publicCallAnswerRepository;
            this._publicCallDeliveryRepository = publicCallDeliveryRepository;
            this._publicCallDocumentRepository = publicCallDocumentRepository;
            this._publicCallFoodRepository = publicCallFoodRepository;
            this._iBGEIntegration = iBGEIntegration;
            this._locationRegionRepository = locationRegionRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(PublicCall publicCall)
        {
            var public_call_id = Guid.NewGuid();
            publicCall.SetId(public_call_id);

            await this._publicCallRepository.Save(publicCall);

            foreach (var food in publicCall.foods)
            {
                var public_call_food_id = Guid.NewGuid();
                food.SetId(public_call_food_id);
                food.SetPublicCallId(public_call_id);
                await this._publicCallFoodRepository.Save(food);
            }

            foreach (var document in publicCall.documents)
            {
                document.SetPublicCallId(public_call_id);
                await this._publicCallDocumentRepository.Save(document);
            }

            await this._publicCallRepository.UnitOfWork.Commit();

            return public_call_id;
        }

        public async Task ChangeStatus(Guid public_call_id, PublicCallStatusEnum status)
        {
            var publicCall = await this._publicCallRepository.Get(public_call_id, true);

            if (publicCall == null)
                return;

            publicCall.SetStatus(status);

            await this._publicCallRepository.Save(publicCall);

            if (status == PublicCallStatusEnum.EmAndamento)
            {
                var publicCallAnswers = await this._publicCallAnswerRepository.GetAllByPublicCallId(public_call_id);

                foreach (var answer in publicCallAnswers)
                {
                    answer.SetAsUnChosen();
                    await this._publicCallAnswerRepository.Save(answer);
                }
            }

            await this._publicCallRepository.UnitOfWork.Commit();
        }

        public async Task<PublicCall?> Get(Guid id, bool keepTrack = false)
        {
           return await this._publicCallRepository.Get(id, keepTrack);
        }

        public async Task<List<PublicCall>> GetAll()
        {
            return await this._publicCallRepository.GetAll();
        }

        public async Task<List<PublicCall>> GetAllDashboard()
        {
            return await this._publicCallRepository.GetAllDashboard();
        }

        public async Task<List<PublicCall>> GetAllOnGoingByCooperative(Guid cooperativeId)
        {
            return await this._publicCallRepository.GetAllOnGoingByCooperative(cooperativeId);
        }

        public async Task<List<PublicCallReport>> GetAllReport(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess)
        {
            var allLocationRegion = await this._locationRegionRepository.GetAll();
            var allCities = await this._iBGEIntegration.GetAllCitiesByStateId(35);
            var reports = await this._publicCallRepository.GetAllReport(startDate, endDate, statusId, filterNameNumberProcess);

            foreach (var report in reports)
            {
                var city = allCities.FirstOrDefault(c => c.municipio.id == report.codigo_cidade_ibge);
                report.municipio_maior_numero_associados = (city?.municipio.nome ?? String.Empty);

                var locationScore = !report.codigo_cidade_ibge.HasValue ? 0 : PublicCallCooperativeDeliveryAggregated.GetLocationScore(report.codigo_cidade_ibge.Value, report.proposal_city_id!.Value, allLocationRegion);

                report.localizacao = locationScore switch
                {
                    1 => "País",
                    2 => "Estado",
                    3 => "Intermediária",
                    4 => "Imediata",
                    5 => "Local",
                    _ => "País"
                };
            }

            return reports;
        }

        public async Task<List<PublicCallReportCooperative>> GetAllReportCooperatives(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess)
        {
            var reports = await this._publicCallRepository.GetAllReportCooperatives(startDate, endDate, statusId, filterNameNumberProcess);

            return reports;
        }

        public async Task<bool> SetAsAwaitingDelivery(Guid id)
        {
            var publicCall = await this._publicCallRepository.Get(id, true);

            if (publicCall == null)
                return false;

            publicCall.SetStatus(PublicCallStatusEnum.Contratada);
            await this._publicCallRepository.Save(publicCall);

            return await this._publicCallRepository.UnitOfWork.Commit();
        }

        public async Task<bool> SetAsBought(Guid id)
        {
            var publicCall = await this._publicCallRepository.Get(id, true);

            if (publicCall == null)
                return false;

            publicCall.SetStatus(PublicCallStatusEnum.Aprovada);
            await this._publicCallRepository.Save(publicCall);

            return await this._publicCallRepository.UnitOfWork.Commit();
        }

        public async Task Update(PublicCall publicCall)
        {
            await this._publicCallRepository.Save(publicCall);

            await this._publicCallFoodRepository.RemoveAllNotInIdList(publicCall.id, publicCall.foods.Select(f => f.id).ToList());

            if (publicCall.foods != null)
                foreach (var food in publicCall.foods)
                {
                    await this._publicCallFoodRepository.Save(food);
                }

            await this._publicCallDocumentRepository.RemoveAll(publicCall.id);

            if (publicCall.documents != null)
                foreach (var document in publicCall.documents)
                {
                    await this._publicCallDocumentRepository.Save(document);
                }

            await this._publicCallRepository.UnitOfWork.Commit();
        }

        public async Task<bool> UpdateIfIsCompleted(Guid id)
        {
            var publicCall = await this._publicCallRepository.Get(id, true);

            if (publicCall == null)
                return false;

            var allDeliveries = await this._publicCallDeliveryRepository.GetAllByPublicCallId(publicCall.id);
            var allDelivered = allDeliveries.All(d => d.was_delivered);

            if (allDelivered)
            {
                publicCall.SetAsCompleted();
                await this._publicCallRepository.Save(publicCall);

                return await this._publicCallRepository.UnitOfWork.Commit();
            }

            return false;
        }

        #endregion [ FIM - Metodos ]
    }
}
