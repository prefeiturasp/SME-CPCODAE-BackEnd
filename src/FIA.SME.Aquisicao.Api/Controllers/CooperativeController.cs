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

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/cooperativa")]
    [ApiController]
    public class CooperativeController : ControllerBase
    {
        private readonly IAuthorizationSMEService _authorizationSMEService;
        private readonly IConfiguration _configuration;
        private readonly IChangeRequestService _changeRequestService;
        private readonly ICooperativeDocumentService _cooperativeDocumentService;
        private readonly ICooperativeMemberService _cooperativeMemberService;
        private readonly ICooperativeService _cooperativeService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IMailService _mailService;
        private readonly IPublicCallAnswerService _publicCallAnswerService;
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;

        public CooperativeController(
            IAuthorizationSMEService authorizationSMEService,
            IConfiguration configuration,
            IChangeRequestService changeRequestService,
            ICooperativeDocumentService cooperativeDocumentService,
            ICooperativeMemberService cooperativeMemberService,
            ICooperativeService cooperativeService,
            IDocumentTypeService documentTypeService,
            IMailService mailService,
            IPublicCallAnswerService publicCallAnswerService,
            IStorageService storageService,
            IUserService userService)
        {
            this._authorizationSMEService = authorizationSMEService;
            this._configuration = configuration;
            this._changeRequestService = changeRequestService;
            this._cooperativeDocumentService = cooperativeDocumentService;
            this._cooperativeService = cooperativeService;
            this._cooperativeMemberService = cooperativeMemberService;
            this._documentTypeService = documentTypeService;
            this._mailService = mailService;
            this._publicCallAnswerService = publicCallAnswerService;
            this._storageService = storageService;
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CooperativeRegister model)
        {
            // Valida que o CNPJ não está cadastrado
            var cnpjExists = await this._cooperativeService.CheckIfCnpjExists(null, model.cnpj);

            if (cnpjExists)
                return new ApiResult(new BadRequestApiResponse("Este CNPJ já está atribuído a outra cooperativa"));

            // Valida que a DAP/CAF não está cadastrada
            var dapCafExists = await this._cooperativeService.CheckIfDapCafExists(null, model.dap_caf_code);

            if (dapCafExists)
            {
                var tipo = model.is_dap ? "DAP" : "CAF";
                return new ApiResult(new BadRequestApiResponse($"Esta {tipo} já está atribuída a outra cooperativa"));
            }

            // Valida que o e-mail não está cadastrado
            var user = await this._userService.Get(model.legal_representative.email);

            if (user != null)
                return new ApiResult(new BadRequestApiResponse("O e-mail do representante legal já está atribuído a outro usuário"));

            // Salva os dados da cooperativa
            var cooperative = model.ToCooperative()!;
            user = model.ToUser()!;
            var id = await this._cooperativeService.Add(cooperative, user);

            // Atualiza os dados da de endereço cooperativa
            cooperative.UpdateAddress(model.address.id, model.address.street, model.address.number, model.address.complement, model.address.district, model.address.cep, model.address.city_id);
            cooperative.UpdateBank(model.bank.id, model.bank.code, model.bank.name, model.bank.agency, model.bank.account_number);
            cooperative.UpdateLegalRepresentative(model.legal_representative.id, model.legal_representative.cpf, model.legal_representative.name, model.legal_representative.phone,
                model.legal_representative.marital_status, model.legal_representative.position, model.legal_representative.position_expiration_date,
                model.legal_representative.address.id, model.legal_representative.address.street, model.legal_representative.address.number, model.legal_representative.address.complement,
                model.legal_representative.address.district, model.legal_representative.address.cep, model.legal_representative.address.city_id);
            await this._cooperativeService.Update(cooperative);

            // Cria o token que será enviado por e-mail
            var token = await this._cooperativeService.CreateRegisterHmacCode(model.cnpj);

            if (String.IsNullOrEmpty(token))
                return new ApiResult(new BadRequestApiResponse("Ocorreu um erro ao tentar criar esta cooperativa"));

            var urlEncodedToken = WebUtility.UrlEncode(token);

            var urlContinueRegistration = $"{this._configuration["Frontend:Url"]}/cooperativa/confirme-email/{urlEncodedToken}";
            _ = this._mailService.SendConfirmRegistrationEmail(user, urlContinueRegistration);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { id = id, token = token }));
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpPost]
        [Route("adicionar-documento")]
        public async Task<IActionResult> AddDocument([FromBody] CooperativeDocumentRegister model)
        {
            // Busca a cooperativa
            var cooperative = await this._cooperativeService.Get(model.cooperative_id);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            // Busca o tipo de documento
            var documentType = await this._documentTypeService.Get(model.document_type_id);

            if (documentType == null)
                return new ApiResult(new BadRequestApiResponse("Tipo de Documento não encontrado"));

            // Verifica se o documento é base 64
            if (String.IsNullOrEmpty(model.file_base_64) || !model.file_base_64.IsBase64String())
                return new ApiResult(new BadRequestApiResponse("Este documento está inválido"));

            // Realiza o upload do arquivo para o azure
            var document_path = await this._storageService.Save(model.cooperative_id, documentType.name, model.file_base_64);

            // Caso tenha sido com sucesso, grava no banco de dados as informações
            var newDocument = new CooperativeDocument(model.cooperative_id, model.document_type_id, model.public_call_id, document_path, model.file_size, null);
            var document_id = await this._cooperativeDocumentService.Add(newDocument);
            newDocument.SetId(document_id);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new CooperativeDocumentResponse(newDocument)));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpGet()]
        [Route("{cooperative_id:guid}/check-is-already-answered/{public_call_id:guid}")]
        public async Task<IActionResult> CheckIfIsAlreadyAnswered(Guid cooperative_id, Guid public_call_id)
        {
            var isAlreadyAnswered = (await this._publicCallAnswerService.GetByCooperativeIdPublicCallId(cooperative_id, public_call_id)) is not null;

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, isAlreadyAnswered));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Busca a cooperativa
            var cooperative = await this._cooperativeService.Get(id, true);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            cooperative.Disable();

            await this._cooperativeService.Update(cooperative);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cooperativa removida com sucesso", new { id }));
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpDelete]
        [Route("{cooperative_id:guid}/remover-documento/{id:guid}")]
        public async Task<IActionResult> DeleteDocument(Guid id, Guid cooperative_id)
        {
            // Busca o documento
            var cooperativeDocument = await this._cooperativeDocumentService.Get(id);

            if (cooperativeDocument == null)
                return new ApiResult(new BadRequestApiResponse("Documento não encontrado"));

            // Atualiza o banco de dados as informações deixando ele como old
            await this._cooperativeDocumentService.Remove(id);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Documento removido com sucesso", true));
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpDelete]
        [Route("{cooperative_id:guid}/remover-cooperado/{id:guid}")]
        public async Task<IActionResult> DeleteMember(Guid id, Guid cooperative_id)
        {
            await this._cooperativeMemberService.Remove(id);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cooperado removido com sucesso", new { id, cooperative_id }));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("email-confirmation/{token}")]
        public async Task<IActionResult> EmailConfirmation(string token)
        {
            var cnpj = await this._cooperativeService.GetCnpjByToken(token);

            if (String.IsNullOrEmpty(cnpj))
                return new ApiResult(new BadRequestApiResponse("O token utilizado está expirado ou inválido. Nenhum CNPJ foi encontrado"));

            var cooperative = await this._cooperativeService.GetByCnpj(cnpj);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("O token utilizado está expirado ou inválido. Nenhuma cooperativa foi encontrada para este CNPJ"));

            if (cooperative.status > CooperativeStatusEnum.AwaitingEmailConfirmation)
                return new ApiResult(new BadRequestApiResponse("Este token já foi utilizado. Realize o login para completar seu cadastro"));

            // Altera o status da cooperativa
            cooperative.SetToCompletedRegistration();
            await this._cooperativeService.Update(cooperative);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "E-mail confirmado com sucesso", new { id = cooperative.id, token = token }));
        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var cooperative = new CooperativeDetailResponse(await this._cooperativeService.Get(id), true);

            if (cooperative != null)
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, cooperative));

            return new ApiResult(new NoContentApiResponse());
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetAllRegistered()
        {
            var cooperatives = (await this._cooperativeService.GetAll()).Where(c => c.status == CooperativeStatusEnum.Registered).Select(c => new CooperativeDetailResponse(c, false)).ToList();

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, cooperatives));
        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("obter-por-usuario/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var cooperative = new CooperativeDetailResponse(await this._cooperativeService.GetByUserId(userId), true);

            if (cooperative != null)
            {
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, cooperative));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("obter-por-usuario-login/{userId:guid}")]
        public async Task<IActionResult> GetByUserIdLogin(Guid userId)
        {
            var cooperative = new CooperativeDetailResponse(await this._cooperativeService.GetByUserId(userId), false);

            if (cooperative != null)
            {
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, cooperative));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpGet()]
        [Route("listar-tipos-documento")]
        public async Task<IActionResult> GetDocumentTypes()
        {
            var documentTypes = (await this._documentTypeService.GetAll())
                                    .Where(d => (DocumentTypeEnum)d.application == DocumentTypeEnum.CadastroCooperativa
                                            || (DocumentTypeEnum)d.application == DocumentTypeEnum.PropostaCadastroCooperativa)
                                    .OrderBy(d => d.name)
                                    .Select(d => new DocumentTypeResponse(d))
                                    .ToList();

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, documentTypes));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpGet()]
        [Route("messages")]
        public async Task<IActionResult> GetMessages()
        {
            var userIdentity = (System.Security.Claims.ClaimsIdentity)User?.Identity;

            if (userIdentity == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 401"));

            var loggedUserId = await this._authorizationSMEService.GetLoggedUserId(userIdentity);

            if (loggedUserId == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 403"));

            var cooperative = await this._cooperativeService.GetByUserId(loggedUserId.Value);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            var changeRequests = (await this._changeRequestService.GetAllByCooperative(cooperative.id)).ToList();

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, changeRequests));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpPatch]
        [Route("salvar-passo-1")]
        public async Task<IActionResult> SaveStep1([FromBody] CooperativeStep1 model)
        {
            // Busca a cooperativa
            var cooperative = await this._cooperativeService.Get(model.id);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            var updatedCooperative = new Cooperative(model.id, cooperative.user_id, model.name, model.acronym, model.email, model.logo, model.phone, model.cnpj, model.cnpj_central, cooperative.is_dap, cooperative.dap_caf_code,
                model.dap_caf_registration_date, model.dap_caf_expiration_date, model.pj_type, model.production_type, cooperative.status, cooperative.terms_use_acceptance_ip, 
                cooperative.terms_use_acceptance_date, true, new List<CooperativeDocument>(), cooperative.members.ToList());

            updatedCooperative.UpdateAddress(cooperative.address?.id, model.address.street, model.address.number, model.address.complement, model.address.district, model.address.cep, model.address.city_id);
            updatedCooperative.UpdateBank(cooperative.bank?.id, model.bank.code, model.bank.name, model.bank.agency, model.bank.account_number);
            updatedCooperative.UpdateLegalRepresentative(cooperative.legal_representative?.id, model.legal_representative.cpf, model.legal_representative.name, model.legal_representative.phone, 
                model.legal_representative.marital_status, model.legal_representative.position, model.legal_representative.position_expiration_date,
                cooperative.legal_representative?.address?.id, model.legal_representative.address.street, model.legal_representative.address.number, model.legal_representative.address.complement,
                model.legal_representative.address.district, model.legal_representative.address.cep, model.legal_representative.address.city_id);

            await this._cooperativeService.Update(updatedCooperative);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cooperativa atualizada com sucesso", cooperative));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpPatch]
        [Route("{cooperative_id:guid}/complete-registration")]
        public async Task<IActionResult> SaveStep2(Guid cooperative_id)
        {
            // Busca a cooperativa
            var cooperative = await this._cooperativeService.Get(cooperative_id);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            cooperative.SetToCompletedRegistration();
            await this._cooperativeService.Update(cooperative);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cooperativa atualizada com sucesso", new CooperativeDetailResponse(cooperative, false)));
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CooperativeUpdate model)
        {
            // Busca a cooperativa
            var cooperative = await this._cooperativeService.Get(model.id, true);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            // Caso tenha mudado de cnpj e o cnpj já esteja atribuído a outra cooperativa
            if (cooperative.cnpj != model.cnpj && await this._cooperativeService.CheckIfCnpjExists(model.id, model.cnpj))
                return new ApiResult(new BadRequestApiResponse("Este CNPJ já está atribuído a outra cooperativa"));

            // Caso tenha mudado de dap e a dap já esteja atribuída a outra cooperativa
            if (cooperative.dap_caf_code != model.dap_caf_code && await this._cooperativeService.CheckIfDapCafExists(model.id, model.dap_caf_code))
            {
                var tipo = model.is_dap ? "DAP" : "CAF";
                return new ApiResult(new BadRequestApiResponse($"Esta {tipo} já está atribuída a outra cooperativa"));
            }

            var updatedCooperative = new Cooperative(model.id, cooperative.user_id, model.name, model.acronym, model.email, model.logo, model.phone, model.cnpj, model.cnpj_central, model.is_dap, model.dap_caf_code,
                model.dap_caf_registration_date, model.dap_caf_expiration_date, model.pj_type, model.production_type, cooperative.status, cooperative.terms_use_acceptance_ip, 
                cooperative.terms_use_acceptance_date, model.is_active, cooperative.documents.ToList(),
                cooperative.members.ToList());

            updatedCooperative.UpdateAddress(cooperative.address.id, model.address.street, model.address.number, model.address.complement, model.address.district, model.address.cep, model.address.city_id);
            
            var bankId = cooperative.bank_id ?? Guid.NewGuid();

            updatedCooperative.UpdateBank(bankId, model.bank.code, model.bank.name, model.bank.agency, model.bank.account_number);
            updatedCooperative.UpdateLegalRepresentative(cooperative.legal_representative.id, model.legal_representative.cpf, model.legal_representative.name, model.legal_representative.phone,
                model.legal_representative.marital_status, model.legal_representative.position, model.legal_representative.position_expiration_date,
                cooperative.legal_representative.address.id, model.legal_representative.address.street, model.legal_representative.address.number, model.legal_representative.address.complement, model.legal_representative.address.district,
                model.legal_representative.address.cep, model.legal_representative.address.city_id);

            await this._cooperativeService.Update(updatedCooperative);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cooperativa atualizada com sucesso", new CooperativeDetailResponse(updatedCooperative, true)));
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        [HttpGet()]
        [Route("teste/{id:guid}/{senha}")]
        public async Task<IActionResult> Teste(Guid id, string senha)
        {
            var password = await this._cooperativeService.GetHash(id, senha);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, password));
        }
    }
}
