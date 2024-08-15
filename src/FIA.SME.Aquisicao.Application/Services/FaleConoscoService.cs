using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IFaleConoscoService
    {
        Task<bool> Add(Contact contact);
        Task<Contact?> Get(Guid id, bool keepTrack);
        Task<List<Contact>> GetAll(Guid? cooperativeId, Guid? publicCallId, DateTime? startDate, DateTime? endDate);
        Task Send(FaleConosco faleConosco);
    }

    internal class FaleConoscoService : IFaleConoscoService
    {
        #region [ Propriedades ]

        private readonly string _contactEmailAddress;
        private readonly IFaleConoscoRepository _faleConoscoRepository;
        private readonly IMailService _mailService;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public FaleConoscoService(IFaleConoscoRepository faleConoscoRepository, IMailService mailService, IConfiguration configuration)
        {
            this._faleConoscoRepository = faleConoscoRepository;
            this._mailService = mailService;
            this._contactEmailAddress = configuration.GetSection("GeneralInfo:ContactEmailAddress").Value!;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<bool> Add(Contact contact)
        {
            await this._faleConoscoRepository.Save(contact);

            return await this._faleConoscoRepository.UnitOfWork.Commit();
        }

        public async Task<Contact?> Get(Guid id, bool keepTrack)
        {
            return await this._faleConoscoRepository.Get(id, keepTrack);
        }

        public async Task<List<Contact>> GetAll(Guid? cooperativeId, Guid? publicCallId, DateTime? startDate, DateTime? endDate)
        {
            if (startDate is not null || endDate is not null)
            {
                if (startDate is null && endDate is not null)
                    startDate = endDate.Value;
                else if (startDate is not null && endDate is null)
                    endDate = startDate < DateTime.Today ? DateTime.Today : startDate;

                startDate = startDate!.Value.Date;
                endDate = endDate!.Value.Date;
            }

            return await this._faleConoscoRepository.GetAll(cooperativeId, publicCallId, startDate, endDate);
        }

        public async Task Send(FaleConosco faleConosco)
        {
            await this._mailService.SendContactEmail(faleConosco.Title, faleConosco.Message, faleConosco.UserName, faleConosco.CooperativeName, faleConosco.PublicCallName, faleConosco.PublicCallNumber, faleConosco.PublicCallProcess, this._contactEmailAddress, new List<string>() { faleConosco.CooperativeEmail, faleConosco.UserEmail });
        }

        #endregion [ FIM - Metodos ]
    }
}
