using FIA.SME.Aquisicao.Api.Extensions;
using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Xml.Linq;
using static FIA.SME.Aquisicao.Api.Models.PublicCallCooperativeInfoResponse;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/chamadas-publicas")]
    [ApiController]
    public class PublicCallController : ControllerBase
    {
        private readonly IAuthorizationSMEService _authorizationSMEService;
        private readonly IChangeRequestService _changeRequestService;
        private readonly ICooperativeDocumentService _cooperativeDocumentService;
        private readonly ICooperativeMemberService _cooperativeMemberService;
        private readonly ICooperativeService _cooperativeService;
        private readonly IFoodService _foodService;
        private readonly IMailService _mailService;
        private readonly IPublicCallService _publicCallService;
        private readonly IPublicCallAnswerService _publicCallAnswerService;
        private readonly IPublicCallDeliveryService _publicCallDeliveryService;
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public PublicCallController(
            IAuthorizationSMEService authorizationSMEService,
            IChangeRequestService changeRequestService,
            ICooperativeDocumentService cooperativeDocumentService,
            ICooperativeMemberService cooperativeMemberService,
            ICooperativeService cooperativeService,
            IFoodService foodService,
            IMailService mailService,
            IPublicCallService publicCallService,
            IPublicCallAnswerService publicCallAnswerService,
            IPublicCallDeliveryService publicCallDeliveryService,
            IStorageService storageService,
            IUserService userService,
            IConfiguration configuration)
        {
            this._authorizationSMEService = authorizationSMEService;
            this._changeRequestService = changeRequestService;
            this._cooperativeDocumentService = cooperativeDocumentService;
            this._cooperativeMemberService = cooperativeMemberService;
            this._cooperativeService = cooperativeService;
            this._foodService = foodService;
            this._mailService = mailService;
            this._publicCallService = publicCallService;
            this._publicCallAnswerService = publicCallAnswerService;
            this._publicCallDeliveryService = publicCallDeliveryService;
            this._storageService = storageService;
            this._userService = userService;
            this._configuration = configuration;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PublicCallRegistrationRequest model)
        {
            // Salva os dados da chamada
            var publicCallAgency = this._configuration.GetSection("GeneralInfo:PublicCallAgencyName").Value;
            var publicCallType = this._configuration.GetSection("GeneralInfo:PublicCallType").Value;
            var publicCall = model.ToPublicCall(publicCallAgency, publicCallType)!;

            foreach (var food in model.foods)
            {
                // Valida que o produto está cadastrado
                var foodExists = await this._foodService.Get(food.food_id, false);

                if (foodExists == null)
                    return new ApiResult(new BadRequestApiResponse("Produto não encontrado"));

                publicCall.AddFood(food.ToPublicCallFoodRegistration()!);
            }

            foreach (var document in model.documents)
            {
                publicCall.AddDocument(new PublicCallDocument(document.document_type_id, document.food_id));
            }

            var id = await this._publicCallService.Add(publicCall);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { id }));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpPost]
        [Route("{public_call_id:guid}/add-answer/{cooperative_id:guid}")]
        public async Task<IActionResult> AddAnswer([FromBody] PublicCallAnswerProposalRequest model, Guid public_call_id, Guid cooperative_id)
        {
            var publicCall = await this._publicCallService.Get(public_call_id, false);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            var cooperative = await this._cooperativeService.Get(cooperative_id, false);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            // Salva os dados da resposta
            var public_call_answer_ids = new List<(string, Guid)>();
            foreach (var item in model.foods)
            {
                var members = item.members.ConvertAll(m => new PublicCallAnswerMemberSimplified(m.id, m.cpf, m.dap_caf_code, m.price, m.quantity));

                //var public_call_answer_id = await this._publicCallAnswerService.Add(public_call_id, cooperative_id, item.food_id,
                //    item.city_id, item.is_organic, item.city_members_total, item.daps_fisicas_total, item.indigenous_community_total,
                //    item.pnra_settlement_total, item.quilombola_community_total, item.other_family_agro_total, members);
                var public_call_answer_id = await this._publicCallAnswerService.Add(public_call_id, cooperative_id, item.food_id,
                    item.city_id, item.is_organic, city_members_total: 0, daps_fisicas_total: 0, item.indigenous_community_total,
                    item.pnra_settlement_total, item.quilombola_community_total, item.other_family_agro_total, members);

                if (public_call_answer_id == Guid.Empty)
                    return new ApiResult(new BadRequestApiResponse("Ocorreu um erro inesperado na criação da resposta"));

                public_call_answer_ids.Add((item.food_name, public_call_answer_id));
                var documentsList = new List<CooperativeDocument>();
                var contador = 0;

                foreach (var document in item.documents)
                {
                    contador++;

                    // Verifica se o documento é base 64
                    if (String.IsNullOrEmpty(document.file_base_64) || !document.file_base_64.IsBase64String())
                        continue;

                    // Realiza o upload do arquivo para o azure
                    var splittedValue = document.file_base_64.Split(',');
                    var fileBase64 = splittedValue.Length > 1 ? splittedValue[1] : document.file_base_64;
                    var prefix = document.application < 3 ? String.Empty : $"{public_call_id}_";
                    var document_name = $"{prefix}{document.document_type_name}_{contador.ToString().PadLeft(4, '0')}";
                    var document_path = await this._storageService.Save(cooperative_id, document_name, fileBase64);

                    if (document_path != null)
                    {
                        var hasDocInOtherFoods = model.foods.Where(x => x.food_id != item.food_id).SelectMany(x => x.documents).ToList().Exists(x => x.document_type_id == document.document_type_id);

                        var cooperativeDocument = new CooperativeDocument(cooperative_id, document.document_type_id, public_call_id, document_path, document.file_size, (hasDocInOtherFoods ? null : item.food_id));
                        documentsList.Add(cooperativeDocument);
                    }
                }

                if (documentsList.Count > 0)
                    await this._publicCallAnswerService.AddDocuments(documentsList);
            }

            var user = await this._userService.Get(cooperative.user_id, false);
            _ = this._mailService.SendConfirmAnswerEmail(user, publicCall.name, publicCall.process, public_call_answer_ids);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { public_call_answer_ids }));
        }

        [Authorize(Roles = "logistica")]
        [HttpPost]
        [Route("add-delivery/{public_call_answer_id:guid}")]
        public async Task<IActionResult> AddDeliveryByLogistic([FromBody] PublicCallConfirmDeliveryInfoRequest model, Guid public_call_answer_id)
        {
            var userIdentity = (System.Security.Claims.ClaimsIdentity)User?.Identity;

            if (userIdentity == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 401"));

            var loggedUserId = await this._authorizationSMEService.GetLoggedUserId(userIdentity);

            if (loggedUserId == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 403"));

            var publicCallAnswer = await this._publicCallAnswerService.Get(public_call_answer_id, false);

            if (publicCallAnswer == null)
                return new ApiResult(new BadRequestApiResponse("Resposta da Chamada Pública não encontrada"));

            var delivery = new PublicCallDeliveryInfo(publicCallAnswer.id, model.delivered_date, model.delivered_quantity);
            delivery.SetDelivery(loggedUserId.Value, model.delivered_date, model.delivered_quantity);
            var deliveryId = await this._publicCallDeliveryService.AddDeliveryInfo(delivery);

            // Checa se a proposta já atingiu 100% de entrega, caso sim, inativa
            var publicCallId = publicCallAnswer.public_call_id;
            var wasCompleted = await this._publicCallService.UpdateIfIsCompleted(publicCallId);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Confirmação de entrega efetuada com sucesso", new { id = deliveryId, publicCallId = publicCallId, wasCompleted = wasCompleted }));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("{public_call_id:guid}/buy")]
        public async Task<IActionResult> Buy([FromBody] PublicCallAnswerChooseRequest model, Guid public_call_id)
        {
            var publicCall = await this._publicCallService.Get(public_call_id, false);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            var ids = model.selectedCooperatives.Select(ans => ans.public_call_answer_id).ToList();
            var allAnswers = await this._publicCallAnswerService.GetAllByIds(ids);

            if (ids.Count != allAnswers.Count)
                return new ApiResult(new BadRequestApiResponse("Nem todas as respostas foram encontradas"));

            //var deliveries = new List<PublicCallDeliveryInfo>();

            foreach (var answer in allAnswers)
            {
                var newQuantity = model.selectedCooperatives.FirstOrDefault(ans => ans.public_call_answer_id == answer.id)?.new_quantity;
                answer.SetAsChosen(newQuantity);

                //foreach (var delivery in model.deliveries)
                //{
                //    deliveries.Add(new PublicCallDeliveryInfo(answer.id, delivery.delivery_date, delivery.delivery_quantity));
                //}
            }

            var success = await this._publicCallAnswerService.Buy(allAnswers, new List<PublicCallDeliveryInfo>());

            if (success)
                await this._publicCallService.SetAsBought(public_call_id);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Compra realizada com sucesso", new { public_call_id }));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("{public_call_id:guid}/change-request/{cooperative_id:guid}/{food_id:guid}")]
        public async Task<IActionResult> ChangeRequest([FromBody] ChangeRequestRegister model, Guid public_call_id, Guid cooperative_id, Guid food_id)
        {
            var publicCall = await this._publicCallService.Get(public_call_id, false);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            var cooperative = await this._cooperativeService.Get(cooperative_id, false);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            var food = await this._foodService.Get(food_id, false);

            if (food == null)
                return new ApiResult(new BadRequestApiResponse("Alimento não encontrado"));

            var user = await this._userService.Get(cooperative.user_id, false);

            if (user == null)
                return new ApiResult(new BadRequestApiResponse("Usuário da Cooperativa não encontrado"));

            var changeRequest = new ChangeRequest(cooperative_id, public_call_id, food_id, model.message, model.title, model.response_date, false, model.requires_new_upload, model.refused_documents);
            var refusedDocuments = await this._changeRequestService.GetAllByPublicCallCooperative(changeRequest.public_call_id, changeRequest.cooperative_id, changeRequest.refused_documents);
            var id = await this._changeRequestService.Add(changeRequest);

            var urlPublicCallAccess = $"{this._configuration["Frontend:Url"]}/cooperativa/chamadas-publicas/{public_call_id}/{food_id}";
            _ = this._mailService.SendChangeRequestEmail(user, model.title, publicCall.process, urlPublicCallAccess, refusedDocuments);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Solicitação de alteração realizada com sucesso", new { id, changeRequest = new ChangeRequestResponse(changeRequest) }));
        }

        [Authorize(Roles = "admin")]
        [HttpPatch]
        [Route("{public_call_id:guid}/change-status/{status_id}")]
        public async Task<IActionResult> ChangeStatus(Guid public_call_id, int status_id)
        {
            var publicCall = await this._publicCallService.Get(public_call_id, false);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            await this._publicCallService.ChangeStatus(public_call_id, (PublicCallStatusEnum)status_id);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Alteração de status realizada com sucesso", new { public_call_id, status_id }));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpPatch]
        [Route("dashboard-cooperative/{public_call_answer_id:guid}/members-list/clear")]
        public async Task<IActionResult> ClearMemberList(Guid public_call_answer_id)
        {
            var publicCallAnswer = await this._publicCallAnswerService.Get(public_call_answer_id, true);

            if (publicCallAnswer == null)
                return new ApiResult(new BadRequestApiResponse("Resposta da Chamada Pública não encontrada"));

            var changeRequest = new ChangeRequest(publicCallAnswer.cooperative_id, publicCallAnswer.public_call_id, publicCallAnswer.food_id, "Cooperativa reenviando planilha", "Reenvio planilha Cooperativa", DateTime.Now, false, true, new List<Guid>(), true);
            var changeRequestId = await this._changeRequestService.Add(changeRequest);

            publicCallAnswer.SetAsInvalid();
            await this._publicCallAnswerService.Update(publicCallAnswer);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Lista de membros apagada com sucesso", new { public_call_answer_id, change_request_id = changeRequestId }));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpPost]
        [Route("confirm-delivery/{public_call_answer_id:guid}")]
        public async Task<IActionResult> ConfirmDeliveryByCooperative(Guid public_call_answer_id)
        {
            var publicCallAnswer = await this._publicCallAnswerService.Get(public_call_answer_id, true);

            if (publicCallAnswer == null)
                return new ApiResult(new BadRequestApiResponse("Resposta da Chamada Pública não encontrada"));

            publicCallAnswer.SetAsConfirmed();
            var success = await this._publicCallAnswerService.Update(publicCallAnswer);

            if (success)
            {
                var allChosenAnswers = await this._publicCallAnswerService.GetAllChosenByPublicCallId(publicCallAnswer.public_call_id);

                if (allChosenAnswers.All(ca => ca.was_confirmed))
                    await this._publicCallService.SetAsAwaitingDelivery(publicCallAnswer.public_call_id);
            }

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Confirmação de entrega realizada com sucesso", new { public_call_answer_id }));
        }

        [Authorize(Roles = "logistica")]
        [HttpPut]
        [Route("confirm-delivery/{id:guid}")]
        public async Task<IActionResult> ConfirmDeliveryByLogistic([FromBody] PublicCallConfirmDeliveryInfoRequest model, Guid id)
        {
            var userIdentity = (System.Security.Claims.ClaimsIdentity)User?.Identity;

            if (userIdentity == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 401"));

            var loggedUserId = await this._authorizationSMEService.GetLoggedUserId(userIdentity);

            if (loggedUserId == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 403"));

            // Busca as informações da entrega
            var delivery = await this._publicCallDeliveryService.Get(id, true);

            if (delivery == null)
                return new ApiResult(new BadRequestApiResponse("Dados da entrega não encontrados"));

            delivery.SetDelivery(loggedUserId.Value, model.delivered_date, model.delivered_quantity);
            await this._publicCallDeliveryService.Update(delivery);

            // Checa se a proposta já atingiu 100% de entrega, caso sim, inativa
            var publicCallId = delivery.answer.public_call_id;
            var wasCompleted = await this._publicCallService.UpdateIfIsCompleted(publicCallId);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Confirmação de entrega efetuada com sucesso", new { id = id, publicCallId = publicCallId, wasCompleted = wasCompleted }));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Busca a chamada
            var publicCall = await this._publicCallService.Get(id, true);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            publicCall.Disable();

            await this._publicCallService.Update(publicCall);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Chamada Pública removida com sucesso", new { id }));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpDelete]
        [Route("dashboard-cooperative/{public_call_answer_id:guid}/remove-proposal")]
        public async Task<IActionResult> DeleteAnswer(Guid public_call_answer_id)
        {
            var publicCallAnswer = await this._publicCallAnswerService.Get(public_call_answer_id, true);

            if (publicCallAnswer == null)
                return new ApiResult(new BadRequestApiResponse("Resposta da Chamada Pública não encontrada"));

            await this._publicCallAnswerService.Delete(public_call_answer_id);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Proposta removida com sucesso", new { public_call_answer_id }));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("suspend/{id:guid}")]
        public async Task<IActionResult> DeleteSuspension(Guid id)
        {
            // Busca a chamada
            var publicCall = await this._publicCallService.Get(id, true);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            publicCall.Suspend();

            await this._publicCallService.Update(publicCall);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Chamada Pública suspensa com sucesso", new { id }));
        }

        [Authorize(Roles = "logistica")]
        [HttpDelete]
        [Route("delete-delivery/{id:guid}")]
        public async Task<IActionResult> DeleteDeliveryByLogistic(Guid id)
        {
            var userIdentity = (System.Security.Claims.ClaimsIdentity)User?.Identity;

            if (userIdentity == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 401"));

            var loggedUserId = await this._authorizationSMEService.GetLoggedUserId(userIdentity);

            if (loggedUserId == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 403"));

            // Busca as informações da entrega
            var delivery = await this._publicCallDeliveryService.Get(id, true);

            if (delivery == null)
                return new ApiResult(new BadRequestApiResponse("Dados da entrega não encontrados"));

            delivery.RemoveDelivery();
            await this._publicCallDeliveryService.Update(delivery);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Remoção de entrega efetuada com sucesso", new { id = id, publicCallId = delivery.answer.public_call_id, wasCompleted = false }));
        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var chamada = new PublicCallDetailResponse(await this._publicCallService.Get(id));

            if (chamada != null)
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, chamada));

            return new ApiResult(new NoContentApiResponse());
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var allPublicCalls = (await this._publicCallService.GetAll()).ToList();
            var chamadas = allPublicCalls.ConvertAll(pc => new PublicCallListResponse(pc));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, chamadas));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("{id:guid}/cooperatives")]
        public async Task<IActionResult> GetAllCooperativesAvailablesForBeChosen(Guid id)
        {
            var cooperatives = (await this._publicCallDeliveryService.GetAllCooperativesAvailablesForBeChosen(id)).ConvertAll(pc => new PublicCallCooperativeInfoResponse(pc));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, cooperatives));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("{public_call_id:guid}/change-request/history")]
        public async Task<IActionResult> GetAllChangeRequestHistory(Guid public_call_id)
        {
            var changeRequests = (await this._changeRequestService.GetAllByPublicCall(public_call_id)).ConvertAll(cr => new ChangeRequestResponse(cr));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, changeRequests));
        }

        [Authorize(Roles = "admin,logistica")]
        [HttpGet()]
        [Route("{id:guid}/delivery-info")]
        public async Task<IActionResult> GetAllCooperativesDeliveryInfo(Guid id)
        {
            var cooperatives = (await this._publicCallDeliveryService.GetAllCooperativesDeliveryInfo(id)).ConvertAll(pc => new PublicCallCooperativeInfoResponse(pc, pc.total_delivered_percentage, pc.deliveries));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, cooperatives));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("{public_call_id:guid}/attachments")]
        public async Task<IActionResult> GetAllCooperativeDocumentsByPublicCall(Guid public_call_id)
        {
            var cooperativeIds = (await this._publicCallAnswerService.GetAllByPublicCallId(public_call_id)).Select(c => c.cooperative_id).Distinct().ToList();
            var documents = (await this._cooperativeDocumentService.GetAllCooperativeDocumentsByPublicCall(public_call_id, cooperativeIds)).ConvertAll(d => new CooperativeDocumentResponse(d));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, documents));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("{public_call_id:guid}/attachments/{cooperative_id:guid}/zip")]
        public async Task<IActionResult> GetAllCooperativeCurrentZippedDocumentsByPublicCall(Guid public_call_id, Guid cooperative_id)
        {
            var zipFileBase64 = (await this._cooperativeDocumentService.GetCooperativeCurrentZippedDocumentsByPublicCall(public_call_id, cooperative_id));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, zipFileBase64));
        }

        [Authorize(Roles = "admin,logistica")]
        [HttpGet()]
        [Route("dashboard")]
        public async Task<IActionResult> GetAllDashboard()
        {
            var chamadas = (await this._publicCallService.GetAllDashboard()).ConvertAll(pc => new PublicCallDetailResponse(pc));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, chamadas));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpGet()]
        [Route("dashboard-cooperative/{cooperative_id:guid}")]
        public async Task<IActionResult> GetAllDashboardCooperative(Guid cooperative_id)
        {
            var chamadas = (await this._publicCallAnswerService.GetAllByCooperativeId(cooperative_id)).ConvertAll(pc => new PublicCallCooperativeListResponse(pc));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, chamadas));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpGet()]
        [Route("dashboard-cooperative/{public_call_answer_id:guid}/delivery-info")]
        public async Task<IActionResult> GetAllDashboardCooperativeDeliveryInfo(Guid public_call_answer_id)
        {
            var deliveryProgressData = (await this._publicCallDeliveryService.GetAllDeliveryInfoByPublicCallAnswerId(public_call_answer_id));
            var deliveryProgress = deliveryProgressData.ConvertAll(d => new CooperativeDeliveryInfoProgressResponse(d.id, d.delivery_date, 0, d.delivery_quantity, d.was_delivered, null, d.delivered_quantity));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, deliveryProgress));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpGet()]
        [Route("dashboard-cooperative/{public_call_answer_id:guid}/members-list")]
        public async Task<IActionResult> GetAllDashboardCooperativeMembersList(Guid public_call_answer_id)
        {
            var membersList = (await this._publicCallDeliveryService.GetAllDashboardCooperativeMembersList(public_call_answer_id)).ConvertAll(m => new PublicCallAnswerMemberSimplifiedModel(m.cpf, m.dap_caf_code, m.price, m.quantity));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, membersList));
        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("{public_call_id:guid}/{cooperative_id:guid}/{food_id:guid}")]
        public async Task<IActionResult> GetData(Guid public_call_id, Guid cooperative_id, Guid food_id)
        {
            var publicCall = await this._publicCallService.Get(public_call_id);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            var cooperative = await this._cooperativeService.Get(cooperative_id);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            var food = await this._foodService.Get(food_id, false);

            if (food == null)
                return new ApiResult(new BadRequestApiResponse("Produto não encontrado"));

            var answer = await this._publicCallAnswerService.GetByCooperativeIdPublicCallIdFoodId(cooperative_id, public_call_id, food_id);

            if (answer == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não participou desta proposta de venda"));

            var members = (await this._publicCallAnswerService.GetAllMembers(answer.id)).ConvertAll(m => new CooperativeAnswerMemberResponse(m));
            var allAvailableDocuments = (await this._cooperativeDocumentService
                                                    .GetAllCooperativeDocumentsByPublicCall(public_call_id, new List<Guid>() { cooperative_id }))
                                                    .Where(rd => (rd.application == 1 || rd.application == 3))
                                                    .ToList();

            var allTypesOfDocument = allAvailableDocuments.Select(d => d.document_type_id).Distinct();
            var documentsAreCurrent = allAvailableDocuments.Where(d => d.is_current).Select(d => d.document_type_id).Distinct();
            var missingDocumentTypes = allTypesOfDocument.Where(d => !documentsAreCurrent.Contains(d));
            var requiredDocuments = allAvailableDocuments
                                            .Where(d => missingDocumentTypes.Contains(d.document_type_id))
                                            .GroupBy(d => d.document_type_id, (key, group) => group.OrderByDescending(d => d.creation_date).FirstOrDefault())
                                            .ToList();

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, new { answer, members, requiredDocuments }));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("attachments/{document_id:guid}/pdf")]
        public async Task<IActionResult> GetDocumentFileBase64(Guid document_id)
        {
            var pdfFileBase64 = (await this._cooperativeDocumentService.GetDocumentFileBase64(document_id));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, pdfFileBase64));
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpGet()]
        [Route("validation-report/{public_call_answer_id:guid}")]
        public async Task<IActionResult> GetValidationMembersReport(Guid public_call_answer_id)
        {
            var members = (await this._publicCallAnswerService.GetAllMembers(public_call_answer_id)).ConvertAll(m => new CooperativeMemberValidatedResponse(m));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, members));
        }

        [Authorize(Roles = "admin")]
        [HttpPatch]
        [Route("review-document/{cooperative_document_id:guid}")]
        public async Task<IActionResult> SetDocumentAsReviewed(Guid cooperative_document_id, [FromBody] CooperativeDocumentSetAsReviewed model)
        {
            var cooperativeDocument = await this._cooperativeDocumentService.Get(cooperative_document_id);

            if (cooperativeDocument == null)
                return new ApiResult(new BadRequestApiResponse("Documento não encontrado"));

            await this._cooperativeDocumentService.SetAsReviewed(cooperative_document_id, model.is_reviewed);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Alteração de status realizada com sucesso", new { cooperative_document_id, model.is_reviewed }));
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PublicCallUpdateRequest model)
        {
            // Valida que a chamada pública existe
            var publicCall = await this._publicCallService.Get(model.id, true);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            // Verifica se ela está em status Aguardando Compra
            if (publicCall.status_id != (int)PublicCallStatusEnum.EmAndamento)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não pode mais ser editada, pois não está mais aguardando compra"));

            // Salva os dados da chamada
            var updatedPublicCall = new PublicCall(model.id, model.city_id, model.number, model.name, publicCall.type, publicCall.agency, model.process, model.registration_start_date,
                                            model.registration_end_date, model.public_session_date, model.public_session_place, model.public_session_url, model.notice_url, model.notice_object,
                                            model.delivery_information, model.extra_information, model.is_active);

            // Valida que o produto está cadastrado
            foreach (var food in model.foods)
            {
                var foodExists = await this._foodService.Get(food.food_id, false);

                if (foodExists == null)
                    return new ApiResult(new BadRequestApiResponse("Produto não encontrado"));

                updatedPublicCall.AddFood(food.ToPublicCallFoodUpdate()!);
            }

            foreach (var document in model.documents)
            {
                updatedPublicCall.AddDocument(new PublicCallDocument(document.id, document.document_type_id, document.food_id, updatedPublicCall.id));
            }

            await this._publicCallService.Update(updatedPublicCall);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Chamada Pública atualizada com sucesso", new PublicCallDetailResponse(updatedPublicCall)));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpPut]
        [Route("{public_call_id:guid}/update-answer/{cooperative_id:guid}")]
        public async Task<IActionResult> UpdateAnswer([FromBody] PublicCallAnswerProposalRequest model, Guid public_call_id, Guid cooperative_id)
        {
            var publicCall = await this._publicCallService.Get(public_call_id, false);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            var cooperative = await this._cooperativeService.Get(cooperative_id, false);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            PublicCallAnswer? currentPublicAnswerId = !String.IsNullOrEmpty(model.change_request_title) ? 
                                                        (PublicCallAnswer?) (await this._publicCallAnswerService.GetByCooperativeIdPublicCallIdFoodId(cooperative_id, public_call_id, model.foods[0].food_id)):
                                                        null;

            // Salva os dados da resposta
            var public_call_answer_ids = new List<Guid>();
            foreach (var item in model.foods)
            {
                var members = item.members.ConvertAll(m => new PublicCallAnswerMemberSimplified(m.id, m.cpf, m.dap_caf_code, m.price, m.quantity));

                if (currentPublicAnswerId != null && item.city_id == 0)
                {
                    item.city_id = currentPublicAnswerId.city_id;
                    item.is_organic = currentPublicAnswerId.is_organic;
                    //item.city_members_total = currentPublicAnswerId.city_members_total;
                    //item.daps_fisicas_total = currentPublicAnswerId.daps_fisicas_total;
                    item.indigenous_community_total = currentPublicAnswerId.indigenous_community_total;
                    item.pnra_settlement_total = currentPublicAnswerId.pnra_settlement_total;
                    item.quilombola_community_total = currentPublicAnswerId.quilombola_community_total;
                    item.other_family_agro_total = currentPublicAnswerId.other_family_agro_total;
                }

                //var public_call_answer_id = await this._publicCallAnswerService.Update(public_call_id, cooperative_id, item.food_id,
                //    item.city_id, item.is_organic, item.city_members_total, item.daps_fisicas_total, item.indigenous_community_total,
                //    item.pnra_settlement_total, item.quilombola_community_total, item.other_family_agro_total,
                //    members, model.change_request_title, model.change_request_message);
                var public_call_answer_id = await this._publicCallAnswerService.Update(public_call_id, cooperative_id, item.food_id,
                    item.city_id, item.is_organic, city_members_total: 0, daps_fisicas_total: 0, item.indigenous_community_total,
                    item.pnra_settlement_total, item.quilombola_community_total, item.other_family_agro_total,
                    members, model.change_request_title, model.change_request_message);

                if (public_call_answer_id == Guid.Empty)
                    return new ApiResult(new BadRequestApiResponse("Ocorreu um erro inesperado na criação da resposta"));

                public_call_answer_ids.Add(public_call_answer_id);
                var documentsList = new List<CooperativeDocument>();
                var contador = 0;

                foreach (var document in item.documents)
                {
                    contador++;

                    // Verifica se o documento é base 64
                    if (String.IsNullOrEmpty(document.file_base_64) || !document.file_base_64.IsBase64String())
                        continue;

                    // Realiza o upload do arquivo para o azure
                    var splittedValue = document.file_base_64.Split(',');
                    var fileBase64 = splittedValue.Length > 1 ? splittedValue[1] : document.file_base_64;
                    var document_name = $"{public_call_id}_{document.document_type_name}_{contador.ToString().PadLeft(4, '0')}";
                    var document_path = await this._storageService.Save(cooperative_id, document_name, fileBase64);

                    if (document_path != null)
                    {
                        var hasDocInOtherFoods = model.foods.Where(x => x.food_id != item.food_id).SelectMany(x => x.documents).ToList().Exists(x => x.document_type_id == document.document_type_id);
                        var cooperativeDocument = new CooperativeDocument(cooperative_id, document.document_type_id, public_call_id, document_path, document.file_size, (hasDocInOtherFoods ? null : item.food_id));
                        documentsList.Add(cooperativeDocument);
                    }
                }

                if (documentsList.Count > 0)
                    await this._publicCallAnswerService.AddDocuments(documentsList);
            }

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { public_call_answer_ids }));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("validate-members/{public_call_answer_id:guid}")]
        public async Task<IActionResult> ValidateMembers([FromBody] PublicCallValidateMembersRequest model, Guid public_call_answer_id)
        {
            var publicCallAnswer = await this._publicCallAnswerService.Get(public_call_answer_id, false);

            if (publicCallAnswer == null)
                return new ApiResult(new BadRequestApiResponse("Resposta da Chamada Pública não encontrada"));

            var result = await this._publicCallAnswerService.ValidateMembers(public_call_answer_id, model.file_base_64);

            if (result.totalValidatedMembers < result.totalMembers)
                return new ApiResult(new Saida((int)HttpStatusCode.OK, false, "Um ou mais cooperados não foram validados", new { public_call_answer_id, total_validated_members = result.totalValidatedMembers, total_members = result.totalMembers }));;

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Validação de cooperados realizada com sucesso", new { public_call_answer_id, total_validated_members = result.totalValidatedMembers, total_members = result.totalMembers }));
        }
    }
}
