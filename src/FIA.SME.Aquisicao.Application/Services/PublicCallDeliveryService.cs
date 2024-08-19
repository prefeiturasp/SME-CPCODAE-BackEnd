using FIA.SME.Aquisicao.Infrastructure.Integrations;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IPublicCallDeliveryService
    {
        Task<Guid?> AddDeliveryInfo(PublicCallDeliveryInfo delivery);
        Task<bool> AddDeliveryInfo(List<PublicCallDeliveryInfo> deliveries);
        Task<PublicCallDeliveryInfo?> Get(Guid id, bool keepTrack);
        Task<List<PublicCallCooperativeToBeChosenAggregated>> GetAllCooperativesAvailablesForBeChosen(Guid publicCallId);
        Task<List<PublicCallCooperativeDeliveryAggregated>> GetAllCooperativesDeliveryInfo(Guid publicCallId);
        Task<List<PublicCallDeliveryInfo>> GetAllDeliveryInfoByPublicCallAnswerId(Guid publicCallAnswerId);
        Task<List<PublicCallAnswerMemberSimplified>> GetAllDashboardCooperativeMembersList(Guid publicCallAnswerId);
        Task Update(PublicCallDeliveryInfo delivery);
    }

    internal class PublicCallDeliveryService : IPublicCallDeliveryService
    {
        #region [ Propriedades ]

        private readonly IPublicCallAnswerRepository _publicCallAnswerRepository;
        private readonly IPublicCallAnswerMemberRepository _publicCallAnswerMemberRepository;
        private readonly IPublicCallDeliveryRepository _publicCallDeliveryRepository;
        private readonly IIBGEIntegration _iBGEIntegration;
        private readonly ILocationRegionRepository _locationRegionRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallDeliveryService(
            IPublicCallAnswerRepository publicCallAnswerRepository,
            IPublicCallAnswerMemberRepository publicCallAnswerMemberRepository,
            IPublicCallDeliveryRepository publicCallDeliveryRepository,
            IIBGEIntegration iBGEIntegration,
            ILocationRegionRepository locationRegionRepository)
        {
            this._publicCallAnswerRepository = publicCallAnswerRepository;
            this._publicCallAnswerMemberRepository = publicCallAnswerMemberRepository;
            this._publicCallDeliveryRepository = publicCallDeliveryRepository;
            this._iBGEIntegration = iBGEIntegration;
            this._locationRegionRepository = locationRegionRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        #region [ Privados ]

        private async Task<List<PublicCallCooperativeDeliveryAggregated>> GetCooperativesDeliveryInfo(Guid publicCallId, IOrderedEnumerable<PublicCallDeliveryInfo> deliveries)
        {
            var cooperatives = new List<PublicCallCooperativeDeliveryAggregated>();
            var allCities = await this._iBGEIntegration.GetAllCitiesByStateId(35);
            var currentAnswerId = Guid.Empty;

            var answersSelected = await this._publicCallAnswerRepository.GetAllChosenByPublicCallId(publicCallId);

            // Agrega a lista de entregas por cooperativa
            foreach (var delivery in deliveries)
            {
                if (delivery.public_call_answer_id != currentAnswerId)
                {
                    var city = allCities.FirstOrDefault(c => c.municipio.id == delivery.answer.city_id);
                    var cooperative = this.SetCooperativeInfo(delivery.answer, city, delivery);

                    if (cooperative is not null)
                        cooperatives.Add(cooperative);

                    currentAnswerId = delivery.public_call_answer_id;
                }
                else
                {
                    var currentCooperative = cooperatives.First(c => c.public_call_answer_id == currentAnswerId);
                    currentCooperative.AddDeliveryInfo(delivery.id, delivery.delivery_date, delivery.delivery_quantity, delivery.was_delivered, delivery.delivered_confirmation_date, delivery.delivered_quantity);
                }
            }

            // Se o total de entregas confirmadas for diferente do total de cooperativas selecionadas, adiciona as cooperativas
            if (answersSelected.Count != cooperatives.Count)
            {
                foreach (var answer in answersSelected.Where(ase => !cooperatives.Any(c => c.public_call_answer_id == ase.id)))
                {
                    var city = allCities.FirstOrDefault(c => c.municipio.id == answer.city_id);
                    var cooperative = this.SetCooperativeInfo(answer, city, null);

                    if (cooperative is not null)
                        cooperatives.Add(cooperative);
                }
            }

            return cooperatives;
        }

        private PublicCallCooperativeDeliveryAggregated SetCooperativeInfo(PublicCallAnswer answer, IBGEDistrict? city, PublicCallDeliveryInfo? delivery)
        {
            var state = city?.municipio?.microrregiao?.mesorregiao.UF;

            var cooperative = new PublicCallCooperativeDeliveryAggregated(answer, (city?.municipio.nome ?? String.Empty), (state?.sigla ?? String.Empty));

            if (delivery is not null)
                cooperative.AddDeliveryInfo(delivery.id, delivery.delivery_date, delivery.delivery_quantity, delivery.was_delivered, delivery.delivered_confirmation_date, delivery.delivered_quantity);

            var totalProposal = answer.quantity_edited ?? answer.quantity;
            cooperative.SetTotalProposal(totalProposal);

            return cooperative;
        }

        private IEnumerable<IPublicCallCooperativeAggregated> SortCooperatives(IEnumerable<IPublicCallCooperativeAggregated> cooperatives)
        {
            // Seta a classificação
            return cooperatives
                   .OrderByDescending(c => c.location_score) // 1- Localização pelo grupo de localização (Local > Imediata > Intermediária > Estado > País)
                   .ThenByDescending(c => c.percentage_inclusiveness) // 2- Critério de inclusividade (indigenas, aborigenes etc) - porcentagem em relação ao todo
                   .ThenByDescending(c => c.proposal_is_organic) // 3- Se é organico ou não (organico > não organico)
                   .ThenByDescending(c => c.percentage_daps_fisicas) // 4- Proporção de daps fisicas / total de cooperados
                   .ThenBy(c => c.cooperative_is_central) // 5- Se é cooperativa do tipo central ou não (singular > central)
                   //.ThenByDescending(c => c.daps_fisicas_total) // 6- Maior número de daps fisicas
                   .ThenBy(c => c.total_price) // 6- Menor preço
                   .ThenBy(c => c.name) // 7- Nome
                   .ToList();
        }

        #endregion [ FIM - Privados ]

        public async Task<Guid?> AddDeliveryInfo(PublicCallDeliveryInfo delivery)
        {
            await this._publicCallDeliveryRepository.Save(delivery);
            var success = await this._publicCallDeliveryRepository.UnitOfWork.Commit();

            return success ? delivery.id : null;
        }

        public async Task<bool> AddDeliveryInfo(List<PublicCallDeliveryInfo> deliveries)
        {
            foreach (var delivery in deliveries)
            {
                await this._publicCallDeliveryRepository.Save(delivery);
            }

            var success = await this._publicCallDeliveryRepository.UnitOfWork.Commit();

            return success;
        }

        public async Task<PublicCallDeliveryInfo?> Get(Guid id, bool keepTrack)
        {
            return await this._publicCallDeliveryRepository.Get(id, keepTrack);
        }

        public async Task<List<PublicCallCooperativeToBeChosenAggregated>> GetAllCooperativesAvailablesForBeChosen(Guid publicCallId)
        {
            var allCities = await this._iBGEIntegration.GetAllCitiesByStateId(35);
            var allLocationRegion = await this._locationRegionRepository.GetAll();
            var cooperativesResult = (await this._publicCallDeliveryRepository.GetAllCooperativesAvailablesForBeChosen(publicCallId));

            var cooperatives = cooperativesResult.ConvertAll(item =>
            {
                var city = allCities.FirstOrDefault(c => c.municipio.id == item.answer.city_id);
                var state = city?.municipio?.microrregiao?.mesorregiao.UF;

                var cooperative = new PublicCallCooperativeToBeChosenAggregated(item.answer, (city?.municipio.nome ?? String.Empty), (state?.sigla ?? String.Empty), item.delivery_quantity, item.food_id);
                cooperative.SetLocationScore(allLocationRegion!);

                return cooperative;
            });

            return SortCooperatives(cooperatives.ToList<IPublicCallCooperativeAggregated>()).Cast<PublicCallCooperativeToBeChosenAggregated>().ToList();
        }

        public async Task<List<PublicCallCooperativeDeliveryAggregated>> GetAllCooperativesDeliveryInfo(Guid publicCallId)
        {
            var allLocationRegion = await this._locationRegionRepository.GetAll();
            var deliveries = (await this._publicCallDeliveryRepository.GetAllCooperativesDeliveryInfo(publicCallId)).OrderBy(d => d.public_call_answer_id).ThenBy(d => d.delivery_date);

            var cooperatives = await GetCooperativesDeliveryInfo(publicCallId, deliveries);

            // Seta os totais (proposto e já entregue)
            foreach (var item in cooperatives)
            {
                var totalDelivered = deliveries.Where(d => d.public_call_answer_id == item.public_call_answer_id).Sum(d => d.delivered_quantity ?? 0);
                item.SetTotalDelivered(totalDelivered);
                item.SetLocationScore(allLocationRegion!);
            }

            return SortCooperatives(cooperatives.ToList<IPublicCallCooperativeAggregated>()).Cast<PublicCallCooperativeDeliveryAggregated>().ToList();
        }

        public async Task<List<PublicCallAnswerMemberSimplified>> GetAllDashboardCooperativeMembersList(Guid publicCallAnswerId)
        {
            return (await this._publicCallAnswerMemberRepository.GetAllByPublicCallAnswerId(publicCallAnswerId, false)).ConvertAll(m => new PublicCallAnswerMemberSimplified(m.member_id, m.member.cpf, m.member.dap_caf_code, m.price, m.quantity));
        }

        public async Task<List<PublicCallDeliveryInfo>> GetAllDeliveryInfoByPublicCallAnswerId(Guid publicCallAnswerId)
        {
            return await this._publicCallDeliveryRepository.GetAllDeliveryInfoByPublicCallAnswerId(publicCallAnswerId);
        }

        public async Task Update(PublicCallDeliveryInfo delivery)
        {
            await this._publicCallDeliveryRepository.Save(delivery);

            var allDeliveries = await this._publicCallDeliveryRepository.GetAllByPublicCallAnswerId(delivery.public_call_answer_id);
            var totalProposal = allDeliveries.Sum(d => d.delivery_quantity);
            var totalDelivered = allDeliveries.Sum(d => d.delivered_quantity) + delivery.delivered_quantity;
            var deliveredAsProposed = totalProposal <= totalDelivered;

            if (deliveredAsProposed)
            {
                var cancelDeliveries = allDeliveries.Where(d => !d.was_delivered && d.id != delivery.id);

                foreach (var cancelDelivery in cancelDeliveries)
                {
                    this._publicCallDeliveryRepository.Delete(cancelDelivery.id);
                }
            }

            await this._publicCallDeliveryRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
