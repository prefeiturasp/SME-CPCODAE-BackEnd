using FIA.SME.Aquisicao.Core.Domain;
using Microsoft.Extensions.Configuration;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IFaleConoscoService
    {
        Task Send(FaleConosco faleConosco);
    }

    internal class FaleConoscoService : IFaleConoscoService
    {

        #region [ Propriedades ]

        private readonly string _contactEmailAddress;
        private readonly IMailService _mailService;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public FaleConoscoService(IMailService mailService, IConfiguration configuration)
        {
            this._mailService = mailService;
            this._contactEmailAddress = configuration.GetSection("GeneralInfo:ContactEmailAddress").Value;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task Send(FaleConosco faleConosco)
        {
            await this._mailService.SendContactEmail(faleConosco.Title, faleConosco.Message, faleConosco.UserName, faleConosco.CooperativeName, this._contactEmailAddress, new List<string>() { faleConosco.CooperativeEmail, faleConosco.UserEmail });
        }

        #endregion [ FIM - Metodos ]
    }
}
