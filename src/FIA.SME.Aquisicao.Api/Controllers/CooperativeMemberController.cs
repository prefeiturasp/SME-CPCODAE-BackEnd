using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/cooperado")]
    [ApiController]
    public class CooperativeMemberController : ControllerBase
    {
        private readonly ICooperativeMemberService _cooperativeMemberService;
        private readonly ICooperativeService _cooperativeService;
        private readonly IConfiguration _configuration;

        public CooperativeMemberController(
            ICooperativeMemberService cooperativeMemberService,
            ICooperativeService cooperativeService,
            IConfiguration configuration)
        {
            this._cooperativeMemberService = cooperativeMemberService;
            this._cooperativeService = cooperativeService;
            this._configuration = configuration;
        }

        //[Authorize(Roles = "admin")]
        //[HttpPost]
        //public async Task<IActionResult> Add([FromBody] CooperativeMemberRegister model)
        //{
        //    // Valida que a DAP/CAF não está cadastrada
        //    var memberByDapCaf = await this._activeDapListService.GetByDapCaf(model.dap_caf_code);

        //    if (memberByDapCaf != null)
        //    {
        //        var tipo = model.is_dap ? "DAP" : "CAF";
        //        return new ApiResult(new BadRequestApiResponse($"Esta {tipo} já está atribuída a outro cooperado"));
        //    }

        //    var member = new CooperativeMember(model.cooperative_id, model.is_dap, model.dap_caf_code, model.name, model.cpf, model.pf_type, model.production_type,
        //        model.dap_caf_registration_date, model.dap_caf_expiration_date, model.is_male, true);

        //    if (model.cooperative_id.HasValue)
        //    {
        //        // Busca a cooperativa
        //        var cooperative = await this._cooperativeService.Get(model.cooperative_id.Value);

        //        if (cooperative == null)
        //            return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

        //        // Verifica se o cooperado já não faz parte desta cooperativa
        //        if (cooperative.members.Any(m => m.dap_caf_code == model.dap_caf_code))
        //            return new ApiResult(new BadRequestApiResponse("Este cooperado já faz parte da sua cooperativa"));

        //        // Verifica se o cooperado faz parte de outra cooperativa
        //        var cooperativeMember = await this._cooperativeMemberService.GetByDapCafCpf(model.dap_caf_code);

        //        if (cooperativeMember != null && cooperativeMember.cooperative_id != model.cooperative_id)
        //            return new ApiResult(new BadRequestApiResponse("Este cooperado já faz parte de outra cooperativa"));

        //        // Valida que a CPF não está cadastrado
        //        var cpfExists = await this._cooperativeMemberService.CheckIfCpfExists(null, model.cpf);

        //        if (cpfExists)
        //            return new ApiResult(new BadRequestApiResponse($"Este CPF já está atribuído a outro cooperado"));

        //        member.UpdateAddress(null, model.address.street, model.address.number, model.address.complement, model.address.district, model.address.cep, model.address.city_id);
        //    }

        //    var id = await this._activeDapListService.Add(member);

        //    return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { id = id, document = new CooperativeMemberResponse(member) }));
        //}

        //[Authorize(Roles = "admin")]
        //[HttpGet()]
        //[Route("obter-por-dap-caf-cpf/{dap_caf_cpf}")]
        //public async Task<IActionResult> GetByDapCafCpf(string dap_caf_cpf)
        //{
        //    var cooperativeMember = await this._cooperativeMemberService.GetByDapCafCpf(dap_caf_cpf);

        //    if (cooperativeMember == null)
        //    {
        //        cooperativeMember = await this._activeDapListService.GetByDapCaf(dap_caf_cpf);
        //    }

        //    if (cooperativeMember != null)
        //    {
        //        return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, new CooperativeMemberResponse(cooperativeMember)));
        //    }

        //    return new ApiResult(new NoContentApiResponse());
        //}

        //[Authorize(Roles = "cooperativa")]
        //[HttpPost]
        //[Route("validate-members-list")]
        //public async Task<IActionResult> ValidateMembersList([FromBody] CooperativeMemberToValidateRequest model)
        //{
        //    if (model.dap_caf_code_list == null || model.dap_caf_code_list.Count == 0)
        //        return new ApiResult(new BadRequestApiResponse("Ao menos um cooperado deve ser adicionado"));

        //    var result = await this._cooperativeMemberService.ValidateMembersList(model.dap_caf_code_list);

        //    return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, result.ConvertAll(c => 
        //            new CooperativeMemberToValidateResponse(c.id, c.cpf, c.dap_caf_code, c.name, c.lineNumber, c.total_year_supplied_value, c.message, c.is_success)))
        //        );
        //}


        //[Authorize(Roles = "admin")]
        //[HttpPut]
        //public async Task<IActionResult> Update([FromBody] CooperativeMemberUpdate model)
        //{
        //    // Busca o cooperado na lista dap
        //    var memberByDap = await this._activeDapListService.Get(model.id, true);

        //    if (memberByDap == null)
        //        return new ApiResult(new BadRequestApiResponse("Cooperado não encontrado"));

        //    // Valida que a DAP não está cadastrada
        //    var memberByDapOther = await this._activeDapListService.GetByDapCaf(model.dap_caf_code);

        //    if (memberByDapOther != null && memberByDap.id != memberByDapOther.id)
        //    {
        //        var tipo = model.is_dap ? "DAP" : "CAF";
        //        return new ApiResult(new BadRequestApiResponse($"Esta {tipo} já está atribuída a outro cooperado"));
        //    }

        //    var updatedMember = new CooperativeMember(model.cooperative_id, model.is_dap, model.dap_caf_code, model.name, model.cpf, model.pf_type.Value, model.production_type.Value,
        //        model.dap_caf_registration_date.Value, model.dap_caf_expiration_date.Value, model.is_male, model.is_active);
        //    updatedMember.SetId(model.id);

        //    if (model.cooperative_id.HasValue)
        //    {
        //        // Busca a cooperativa
        //        var cooperative = await this._cooperativeService.Get(model.cooperative_id.Value);

        //        if (cooperative == null)
        //            return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

        //        // Valida que a CPF não está cadastrado
        //        var cpfExists = await this._cooperativeMemberService.CheckIfCpfExists(model.id, model.cpf);

        //        if (cpfExists)
        //            return new ApiResult(new BadRequestApiResponse($"Este CPF já está atribuído a outro cooperado"));

        //        // Busca o cooperado já gravado
        //        var member = await this._cooperativeMemberService.Get(model.id, false);

        //        updatedMember.UpdateAddress(member?.address_id, model.address.street, model.address.number, model.address.complement, model.address.district, model.address.cep, model.address.city_id);
        //    }

        //    await this._activeDapListService.Update(updatedMember);

        //    return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cooperado atualizado com sucesso", new CooperativeMemberResponse(memberByDap)));
        //}
    }
}
