using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IFaleConoscoRepository : IRepository
    {
        Task<Contact?> Get(Guid id, bool keepTrack);
        Task<List<Contact>> GetAll(Guid? cooperativeId, Guid? publicCallId, DateTime? startDate, DateTime? endDate);
        Task Save(Contact contact);
    }

    internal class FaleConoscoRepository : IFaleConoscoRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public FaleConoscoRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Contact?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.FaleConosco!
                                        .Include(fc => fc.Cooperativa)
                                        .Include(fc => fc.ChamadaPublica)
                                        .Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var contact = await query.FirstOrDefaultAsync();

            return (contact != null) ? new Contact(contact) : null;
        }

        public async Task<List<Contact>> GetAll(Guid? cooperativeId, Guid? publicCallId, DateTime? startDate, DateTime? endDate)
        {
            var faleConoscos = await this._context.FaleConosco
                                            .Include(fc => fc.Cooperativa)
                                            .Include(fc => fc.ChamadaPublica)
                                            .AsNoTracking()
                                            .Where(fc =>
                                                (cooperativeId == null || fc.cooperativa_id == cooperativeId)
                                                && (publicCallId == null || fc.chamada_publica_id == publicCallId)
                                                && (startDate == null || (fc.data_criacao >= startDate && fc.data_criacao <= endDate))
                                            )
                                            .OrderByDescending(fc => fc.data_criacao)
                                            .ToListAsync();

            return faleConoscos.ConvertAll(fc => new Contact(fc)).ToList();
        }

        public async Task Save(Contact contact)
        {
            var toSave = await this._context.FaleConosco.FirstOrDefaultAsync(c => c.id == contact.id);

            if (toSave == null)
            {
                toSave = new FaleConosco();
                this._context.FaleConosco.Add(toSave);
            }

            toSave.id = contact.id;
            toSave.cooperativa_id = contact.cooperative_id;
            toSave.chamada_publica_id = contact.public_call_id;
            toSave.assunto = contact.subject;
            toSave.mensagem = contact.message;
            toSave.data_criacao = contact.creation_date.SetKindUtc();
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
