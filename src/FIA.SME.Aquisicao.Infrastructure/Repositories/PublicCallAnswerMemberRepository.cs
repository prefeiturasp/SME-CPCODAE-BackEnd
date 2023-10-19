using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IPublicCallAnswerMemberRepository : IRepository
    {
        Task Delete(PublicCallAnswerMember member);
        Task DeleteByPublicCallAnswerId(Guid publicCallAnswerId);
        Task<List<PublicCallAnswerMember>> GetAllByPublicCallAnswerId(Guid publicCallAnswerId, bool keepTrack);
        Task Save(PublicCallAnswerMember answerMember);
    }

    internal class PublicCallAnswerMemberRepository : IPublicCallAnswerMemberRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallAnswerMemberRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task Delete(PublicCallAnswerMember member)
        {
            var cooperado = this._context.ChamadaPublicaRespostaCooperado.FirstOrDefault(c => c.id == member.id);

            if (cooperado != null)
                this._context.ChamadaPublicaRespostaCooperado.Remove(cooperado);
        }

        public async Task DeleteByPublicCallAnswerId(Guid publicCallAnswerId)
        {
            var cooperados = this._context.ChamadaPublicaRespostaCooperado.Where(c => c.chamada_publica_resposta_id == publicCallAnswerId);

            foreach (var cooperado in cooperados)
            {
                this._context.ChamadaPublicaRespostaCooperado.Remove(cooperado);
            }
        }

        public async Task<List<PublicCallAnswerMember>> GetAllByPublicCallAnswerId(Guid publicCallAnswerId, bool keepTrack)
        {
            var query = this._context.ChamadaPublicaRespostaCooperado.Include(cprc => cprc.Cooperado).Where(cprc => cprc.chamada_publica_resposta_id == publicCallAnswerId);

            if (!keepTrack)
                query = query.AsNoTracking();

            var members = query.Select(cprc => new PublicCallAnswerMember(cprc)).ToList();

            return members;
        }

        public async Task Save(PublicCallAnswerMember answerMember)
        {
            var toSave = await this._context.ChamadaPublicaRespostaCooperado.FirstOrDefaultAsync(c => c.id == answerMember.id);

            if (toSave == null)
            {
                toSave = new ChamadaPublicaRespostaCooperado();
                this._context.ChamadaPublicaRespostaCooperado.Add(toSave);
            }

            toSave.id = answerMember.id;
            toSave.chamada_publica_resposta_id = answerMember.public_call_answer_id;
            toSave.cooperado_id = answerMember.member_id;
            toSave.preco = answerMember.price;
            toSave.quantidade = answerMember.quantity;
            toSave.validado = answerMember.validated;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
