using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Extensions
{
    public static class ToEntityExtension
    {
        #region [ Cooperative ]

        public static Cooperative? ToCooperative(this CooperativeRegister model)
        {
            if (model == null)
                return null;

            var cooperative = new Cooperative(model.id, Guid.Empty, model.name, model.acronym, model.email, model.phone, model.cnpj, model.cnpj_central, model.is_dap, model.dap_caf_code,
                model.dap_caf_registration_date, model.dap_caf_expiration_date, model.pj_type, model.production_type, CooperativeStatusEnum.AwaitingEmailConfirmation, model.terms_use_acceptance_ip,
                DateTime.UtcNow, true, new List<CooperativeDocument>(), new List<CooperativeMember>());

            return cooperative;
        }

        public static User? ToUser(this CooperativeRegister model)
        {
            if (model == null)
                return null;

            var user = new User(model.legal_representative.name, model.legal_representative.email, model.password);

            return user;
        }

        #endregion [ FIM - Cooperative ]

        #region [ PublicCall ]

        public static PublicCall? ToPublicCall(this PublicCallRegistrationRequest model, string agency, string type)
        {
            if (model == null)
                return null;

            var publicCall = new PublicCall(Guid.NewGuid(), model.city_id, model.number, model.name, type, agency, model.process, model.registration_start_date, model.registration_end_date,
                                            model.public_session_date, model.public_session_place, model.public_session_url, model.notice_url, model.notice_object, model.delivery_information,
                                            model.extra_information, true);

            return publicCall;
        }

        public static PublicCallFood? ToPublicCallFoodRegistration(this PublicCallFoodRegistrationRequest model)
        {
            if (model == null)
                return null;

            var food = new PublicCallFood(Guid.NewGuid(), model.food_id, Guid.NewGuid(), model.price, (MeasureUnitEnum)model.measure_unit_id, model.quantity, model.accepts_organic, model.is_organic, true);

            return food;
        }

        public static PublicCallFood? ToPublicCallFoodUpdate(this PublicCallFoodUpdateRequest model)
        {
            if (model == null)
                return null;

            var food = new PublicCallFood(model.id, model.food_id, model.public_call_id, model.price, (MeasureUnitEnum)model.measure_unit_id, model.quantity, model.accepts_organic, model.is_organic, true);

            return food;
        }

        #endregion [ FIM - PublicCall ]

        #region [ User ]

        public static User? ToUser(this UserRegister model)
        {
            if (model == null)
                return null;

            var user = new User(model.name, model.email, model.password);

            var role = model.role == RoleEnum.Cooperativa.ToString().ToLower() ? RoleEnum.Cooperativa : RoleEnum.Admin;
            user.SetRole(role);

            return user;
        }

        #endregion [ FIM - User ]
    }
}
