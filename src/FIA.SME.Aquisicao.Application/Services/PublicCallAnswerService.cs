using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Components;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IPublicCallAnswerService
    {
        Task<Guid> Add(Guid public_call_id, Guid cooperative_id, Guid food_id, int city_id, bool is_organic, int city_members_total, int daps_fisicas_total, int indigenous_community_total, int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, List<PublicCallAnswerMemberSimplified> membersList);
        Task AddDocuments(List<CooperativeDocument> documents);
        Task<bool> Buy(List<PublicCallAnswer> answers, List<PublicCallDeliveryInfo> deliveries);
        Task Delete(Guid publicCallAnswerId);
        Task<PublicCallAnswer?> Get(Guid id, bool keepTrack);
        Task<PublicCallAnswer?> GetByCooperativeIdPublicCallId(Guid cooperativeId, Guid publicCallId);
        Task<PublicCallAnswer?> GetByCooperativeIdPublicCallIdFoodId(Guid cooperativeId, Guid publicCallId, Guid foodId);
        Task<List<PublicCallAnswer>> GetAllByCooperativeId(Guid cooperativeId);
        Task<List<PublicCallAnswer>> GetAllByIds(List<Guid> ids);
        Task<List<PublicCallAnswer>> GetAllByPublicCallId(Guid publicCallId);
        Task<List<PublicCallAnswer>> GetAllChosenByPublicCallId(Guid publicCallId);
        Task<List<PublicCallAnswerMember>> GetAllMembers(Guid publicCallAnswerId);
        Task<bool> Update(PublicCallAnswer answer);
        Task<Guid> Update(Guid public_call_id, Guid cooperative_id, Guid food_id, int city_id, bool is_organic, int city_members_total, int daps_fisicas_total, int indigenous_community_total, int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, List<PublicCallAnswerMemberSimplified> membersList, string change_request_title, string change_request_message);
        Task<(int totalValidatedMembers, int totalMembers)> ValidateMembers(Guid publicCallAnswerId, string dapExtractFileBase64);
    }

    internal class PublicCallAnswerService : IPublicCallAnswerService
    {
        #region [ Propriedades ]

        private readonly IChangeRequestRepository _changeRequestRepository;
        private readonly ICsvParserComponent _csvParserComponent;
        private readonly ICooperativeDocumentRepository _cooperativeDocumentRepository;
        private readonly ICooperativeDocumentService _cooperativeDocumentService;
        private readonly ICooperativeMemberRepository _cooperativeMemberRepository;
        private readonly IPublicCallAnswerRepository _publicCallAnswerRepository;
        private readonly IPublicCallAnswerMemberRepository _publicCallAnswerMemberRepository;
        private readonly IPublicCallDeliveryRepository _publicCallDeliveryRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallAnswerService(
            IChangeRequestRepository changeRequestRepository,
            ICsvParserComponent cSVParserComponent,
            ICooperativeDocumentRepository cooperativeDocumentRepository,
            ICooperativeDocumentService cooperativeDocumentService,
            ICooperativeMemberRepository cooperativeMemberRepository,
            IPublicCallAnswerRepository publicCallAnswerRepository,
            IPublicCallAnswerMemberRepository publicCallAnswerMemberRepository,
            IPublicCallDeliveryRepository publicCallDeliveryRepository
        )
        {
            this._changeRequestRepository = changeRequestRepository;
            this._csvParserComponent = cSVParserComponent;
            this._cooperativeDocumentRepository = cooperativeDocumentRepository;
            this._cooperativeDocumentService = cooperativeDocumentService;
            this._cooperativeMemberRepository = cooperativeMemberRepository;
            this._publicCallAnswerRepository = publicCallAnswerRepository;
            this._publicCallAnswerMemberRepository = publicCallAnswerMemberRepository;
            this._publicCallDeliveryRepository = publicCallDeliveryRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(Guid public_call_id, Guid cooperative_id, Guid food_id, int city_id, bool is_organic, int city_members_total, int daps_fisicas_total, int indigenous_community_total,
            int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, List<PublicCallAnswerMemberSimplified> membersList)
        {
            var public_call_answer_id = Guid.NewGuid();

            return await SaveWithMemberList(public_call_id, cooperative_id, public_call_answer_id, food_id, city_id, is_organic, city_members_total, daps_fisicas_total, indigenous_community_total, pnra_settlement_total,
                quilombola_community_total, other_family_agro_total, membersList);
        }

        public async Task AddDocuments(List<CooperativeDocument> documents)
        {
            if (documents.Count <= 0)
                return;

            var cooperative_id = documents[0].cooperative_id;
            var documentsByType = (await this._cooperativeDocumentService.GetAll(cooperative_id)).Where(d => d.is_current);

            foreach (var document in documents)
            {
                // Se já existir um documento deste tipo, deixa como desabilitado
                var existingDocument = documentsByType.FirstOrDefault(d => d.document_type_id == document.document_type_id);

                if (existingDocument != null)
                    await this._cooperativeDocumentService.Remove(existingDocument.id);

                await this._cooperativeDocumentService.Add(document);
            }
        }

        public async Task<bool> Buy(List<PublicCallAnswer> answers, List<PublicCallDeliveryInfo> deliveries)
        {
            foreach (var answer in answers)
            {
                await this._publicCallAnswerRepository.Save(answer);
            }

            //foreach (var delivery in deliveries)
            //{
            //    await this._publicCallDeliveryRepository.Save(delivery);
            //}

            return await this._publicCallAnswerRepository.UnitOfWork.Commit();
        }

        public async Task Delete(Guid publicCallAnswerId)
        {
            var publicCallAnswer = await this._publicCallAnswerRepository.Get(publicCallAnswerId, false);

            if (publicCallAnswer == null)
                return;

            await this._publicCallAnswerMemberRepository.DeleteByPublicCallAnswerId(publicCallAnswerId);
            await this._publicCallDeliveryRepository.DeleteByPublicCallAnswerId(publicCallAnswerId);
            await this._cooperativeDocumentRepository.DeleteByPublicCallIdCooperativeId(publicCallAnswer.public_call_id, publicCallAnswer.cooperative_id);
            await this._publicCallAnswerRepository.Delete(publicCallAnswerId);

            await this._publicCallAnswerRepository.UnitOfWork.Commit();
        }

        public async Task<PublicCallAnswer?> Get(Guid id, bool keepTrack)
        {
            return await this._publicCallAnswerRepository.Get(id, keepTrack);
        }

        public async Task<PublicCallAnswer?> GetByCooperativeIdPublicCallId(Guid cooperativeId, Guid publicCallId)
        {
            return await this._publicCallAnswerRepository.GetByCooperativeIdPublicCallId(cooperativeId, publicCallId);
        }

        public async Task<PublicCallAnswer?> GetByCooperativeIdPublicCallIdFoodId(Guid cooperativeId, Guid publicCallId, Guid foodId)
        {
            return await this._publicCallAnswerRepository.GetByCooperativeIdPublicCallIdFoodId(cooperativeId, publicCallId, foodId);
        }

        public async Task<List<PublicCallAnswer>> GetAllByCooperativeId(Guid cooperativeId)
        {
            var publicCallAnswers = await this._publicCallAnswerRepository.GetAllByCooperativeId(cooperativeId);
            return publicCallAnswers;
        }

        public async Task<List<PublicCallAnswer>> GetAllByIds(List<Guid> ids)
        {
            return await this._publicCallAnswerRepository.GetAllByIds(ids);
        }

        public async Task<List<PublicCallAnswer>> GetAllByPublicCallId(Guid publicCallId)
        {
            return await this._publicCallAnswerRepository.GetAllByPublicCallId(publicCallId);
        }

        public async Task<List<PublicCallAnswer>> GetAllChosenByPublicCallId(Guid publicCallId)
        {
            return await this._publicCallAnswerRepository.GetAllChosenByPublicCallId(publicCallId);
        }

        public async Task<List<PublicCallAnswerMember>> GetAllMembers(Guid publicCallAnswerId)
        {
            var publicCallAnswerMembers = (await this._publicCallAnswerMemberRepository.GetAllByPublicCallAnswerId(publicCallAnswerId, false)).Where(m => m != null).ToList();
            return publicCallAnswerMembers;
        }

        private async Task<Guid> Save(Guid public_call_id, Guid cooperative_id, Guid public_call_answer_id, Guid food_id, int city_id, bool is_organic, int city_members_total, int daps_fisicas_total, int indigenous_community_total,
            int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, decimal price, decimal quantity)
        {
            var public_call_answer = new PublicCallAnswer(public_call_answer_id, cooperative_id, food_id, public_call_id, city_id, price, quantity, is_organic, city_members_total, daps_fisicas_total,
                indigenous_community_total, pnra_settlement_total, quilombola_community_total, other_family_agro_total);

            // Salva os dados da resposta
            await this._publicCallAnswerRepository.Save(public_call_answer);
            var sucesso = await this._publicCallAnswerMemberRepository.UnitOfWork.Commit();

            return sucesso ? public_call_answer_id : Guid.Empty;
        }

        private async Task<Guid> SaveWithMemberList(Guid public_call_id, Guid cooperative_id, Guid public_call_answer_id, Guid food_id, int city_id, bool is_organic, int city_members_total, int daps_fisicas_total, int indigenous_community_total,
            int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, List<PublicCallAnswerMemberSimplified> membersList)
        {
            // Cadastra as daps/cafs ainda não cadastradas
            var dapCafList = membersList.Where(m => m.id is null).Select(m => new { m.id, m.dap_caf_code, m.cpf }).Distinct().ToList();
            var membersAlreadyRegisteredList = await this._cooperativeMemberRepository.GetAllByDapCaf(dapCafList.Select(m => m.dap_caf_code).ToList());
            var newMembers = new List<CooperativeMember>();

            foreach (var dapCaf in dapCafList.Where(d => !membersAlreadyRegisteredList.Any(m => m.dap_caf_code == d.dap_caf_code)))
            {
                var member = new CooperativeMember(cooperative_id, dapCaf.dap_caf_code, dapCaf.cpf?.ToOnlyNumbers());
                newMembers.Add(member);

                await this._cooperativeMemberRepository.Save(member);
            }

            var members = membersList.ConvertAll(m =>
            {
                if (!m.id.HasValue)
                {
                    var member = newMembers.FirstOrDefault(n => n.dap_caf_code.Equals(m.dap_caf_code, StringComparison.InvariantCultureIgnoreCase));
                    var newId = member != null ? member.id : membersAlreadyRegisteredList.FirstOrDefault(dc => dc.dap_caf_code == m.dap_caf_code)?.id ?? Guid.NewGuid();
                    m.SetId(newId);
                }

                var memberId = m.id;

                return new PublicCallAnswerMember(memberId.Value, m.price, m.quantity);
            });

            // Calcula os valores totalizados
            var price = members.Sum(m => m.quantity * m.price);
            var quantity = members.Sum(m => m.quantity);

            foreach (var member in members)
            {
                var public_call_member_id = Guid.NewGuid();
                member.SetIds(public_call_member_id, public_call_answer_id);

                await this._publicCallAnswerMemberRepository.Save(member);
            }

            var result = await Save(public_call_id, cooperative_id, public_call_answer_id, food_id, city_id, is_organic, city_members_total, daps_fisicas_total, indigenous_community_total,
                    pnra_settlement_total, quilombola_community_total, other_family_agro_total, price, quantity);

            return result;
        }

        public async Task<bool> Update(PublicCallAnswer answer)
        {
            await this._publicCallAnswerRepository.Save(answer);
            var success = await this._publicCallAnswerRepository.UnitOfWork.Commit();

            return success;
        }

        public async Task<Guid> Update(Guid public_call_id, Guid cooperative_id, Guid food_id, int city_id, bool is_organic, int city_members_total, int daps_fisicas_total, int indigenous_community_total,
            int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, List<PublicCallAnswerMemberSimplified> membersList, string change_request_title,
            string change_request_message)
        {
            var publicCallAnswer = await this._publicCallAnswerRepository.GetByCooperativeIdPublicCallIdFoodId(cooperative_id, public_call_id, food_id);

            var changeRequest = new ChangeRequest(cooperative_id, public_call_id, food_id, change_request_message, change_request_title, DateTime.UtcNow, true, false, new List<Guid>());
            await this._changeRequestRepository.Save(changeRequest);

            // Se tiver que trocar os cooperados, apaga a lista anterior
            if (membersList.Count > 0)
            {
                var allMembers = await this._publicCallAnswerMemberRepository.GetAllByPublicCallAnswerId(publicCallAnswer!.id, false);

                foreach (var member in allMembers)
                {
                    await this._publicCallAnswerMemberRepository.Delete(member);
                }

                return await SaveWithMemberList(public_call_id, cooperative_id, publicCallAnswer!.id, food_id, city_id, is_organic, city_members_total, daps_fisicas_total, indigenous_community_total, pnra_settlement_total,
                quilombola_community_total, other_family_agro_total, membersList);
            }

            return await Save(public_call_id, cooperative_id, publicCallAnswer!.id, food_id, city_id, is_organic, city_members_total, daps_fisicas_total, indigenous_community_total, pnra_settlement_total,
                quilombola_community_total, other_family_agro_total, publicCallAnswer.price, publicCallAnswer.quantity);
        }

        public async Task<(int totalValidatedMembers, int totalMembers)> ValidateMembers(Guid publicCallAnswerId, string dapExtractFileBase64)
        {
            var dapExtractedRecords = await this._csvParserComponent.ParseDapCafExtract(dapExtractFileBase64);
            var publicCallAnswerMembers = await this._publicCallAnswerMemberRepository.GetAllByPublicCallAnswerId(publicCallAnswerId, true);
            var publicCallAnswer = await this._publicCallAnswerRepository.Get(publicCallAnswerId, true);
            var totalValidatedMembers = 0;

            foreach (var publicCallAnswerMember in publicCallAnswerMembers)
            {
                var record = dapExtractedRecords.FirstOrDefault(r => r.dap.Equals(publicCallAnswerMember.member.dap_caf_code));

                if (record == null)
                    continue;

                var expirationDate = record.validade.StringToDateTime();
                publicCallAnswerMember.member.SetData(record.cpf.ToOnlyNumbers(), record.nome, expirationDate);
                publicCallAnswerMember.SetIsValidated();

                await this._publicCallAnswerMemberRepository.Save(publicCallAnswerMember);
                await this._cooperativeMemberRepository.Save(publicCallAnswerMember.member);
                totalValidatedMembers++;
            }

            var allMembersWereValidated = totalValidatedMembers == publicCallAnswerMembers.Count;

            publicCallAnswer!.SetWasValidated(allMembersWereValidated);
            await this._publicCallAnswerRepository.Save(publicCallAnswer);

            var success = await this._publicCallAnswerMemberRepository.UnitOfWork.Commit();

            return (totalValidatedMembers, publicCallAnswerMembers.Count);
        }

        #endregion [ FIM - Metodos ]
    }
}
