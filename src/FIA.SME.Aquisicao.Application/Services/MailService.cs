using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Components;
using FIA.SME.Aquisicao.Infrastructure.Models;
using System.Text;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IMailService
    {
        Task SendChangePasswordEmail(User user, string password, string urlRecoverPassword);
        Task SendChangeRequestEmail(User user, string title, string processNumber, string urlPublicCall, List<CooperativeDocument> refusedDocuments);
        Task SendConfirmAnswerEmail(User user, string publicCallName, string publicCallProcessNumber, List<(string, Guid)> foods);
        Task SendConfirmRegistrationEmail(User user, string urlContinueRegistration);
        Task SendContactEmail(string title, string message, string userName, string cooperativeName, string to, List<string> cc);
        Task SendRecoverPasswordEmail(User user, string urlRecoverPassword);
    }

    internal class MailService : IMailService
    {
        private readonly IMailComponent _mailComponent;

        public MailService(IMailComponent mailComponent)
        {
            this._mailComponent = mailComponent;
        }

        public async Task SendChangePasswordEmail(User user, string password, string urlRecoverPassword)
        {
            var subject = "Redefina sua senha";
            var body = new StringBuilder();

            body.Append("<div style='font-size: 0.9rem;'>");

            body.Append($"Olá, {user.name.GetFirstWord()}!");
            body.Append("<br /><br /><br />");
            body.Append($"Sua senha temporária é: <b>{password}</b>");
            body.Append("<br /><br />");
            body.Append($"<a href='{urlRecoverPassword}'>Clique aqui</a> para redefinir sua senha.");
            body.Append("<br /><br /><br />");
            body.Append("Se você tiver algum problema com o link acima, clique com o botão direito nele e copie a URL no navegador.");
            body.Append("<br /><br />");
            body.Append("E-mail enviado automaticamente, favor não responder");

            body.Append("</div>");

            await this._mailComponent.SendEmail(user.email, user.name, subject, body.ToString());
        }

        public async Task SendChangeRequestEmail(User user, string title, string processNumber, string urlPublicCall, List<CooperativeDocument> refusedDocuments)
        {
            var subject = $"Solicitação de Alteração  - Chamada Pública: {processNumber}";
            var body = new StringBuilder();

            body.Append("<div style='font-size: 0.9rem;'>");

            body.Append($"Olá, {user.name.GetFirstWord()}!");
            body.Append("<br /><br /><br />");
            body.Append($"Você recebeu uma solicitação de alteração em uma chamada pública que está participando: <b>{title}</b>");
            body.Append("<br /><br />");

            if (refusedDocuments != null && refusedDocuments.Any())
            {
                body.Append("Segue abaixo a lista com os documentos que necessitam revisão:");

                foreach (var document in refusedDocuments)
                {
                    body.Append("<ul>");
                    body.Append($"<li>{document.document_type_name}</li>");
                    body.Append("</ul>");
                }

                body.Append("<br /><br />");
            }

            body.Append($"<a href='{urlPublicCall}'>Clique aqui</a> para acessar a chamada.");
            body.Append("<br /><br /><br />");
            body.Append("Se você tiver algum problema com o link acima, clique com o botão direito nele e copie a URL no navegador.");
            body.Append("<br /><br />");
            body.Append("E-mail enviado automaticamente, favor não responder");

            body.Append("</div>");

            await this._mailComponent.SendEmail(user.email, user.name, subject, body.ToString());
        }

        public async Task SendConfirmAnswerEmail(User user, string publicCallName, string publicCallProcessNumber, List<(string, Guid)> foods)
        {
            var subject = $"Confirmação de Participação na chamada {publicCallName}";
            var body = new StringBuilder();

            body.Append("<div style='font-size: 0.9rem;'>");

            body.Append($"Olá, {user.name.GetFirstWord()}!");
            body.Append("<br /><br /><br />");
            body.Append($"Este é um e-mail de confirmação de subscrição na chamada {publicCallName}. Processo nro: {publicCallProcessNumber}");
            body.Append("<br /><br />");

            body.Append("<ul>");

            foreach (var item in foods)
            {
                body.Append($"<li>{item.Item1}: Código: {item.Item2}</li>");
            }

            body.Append("</ul>");

            body.Append("<br /><br />");
            body.Append("O envio da documentação para as chamadas públicas ocorrerá por meio do Sistema AF-CODAE. Esclarecemos, contudo, que o Diário Oficial da Cidade de São Paulo permanece como o veículo oficial de comunicação sobre os processos administrativos. Acompanhe a publicação das atas referentes às chamadas públicas de seu interesse.");
            body.Append("<br /><br /><br />");
            body.Append("E-mail enviado automaticamente, favor não responder");

            body.Append("</div>");

            await this._mailComponent.SendEmail(user.email, user.name, subject, body.ToString());
        }

        public async Task SendConfirmRegistrationEmail(User user, string urlContinueRegistration)
        {
            var subject = "Confirme seu e-mail";
            var body = new StringBuilder();

            body.Append("<div style='font-size: 0.9rem;'>");

            body.Append($"Olá, {user.name.GetFirstWord()}!");
            body.Append("<br /><br /><br />");
            body.Append("Para acessar o sistema, clique no link abaixo!");
            body.Append("<br /><br />");
            body.Append($"<a href='{urlContinueRegistration}'>Clique aqui</a> para confirmar seu e-mail.");
            body.Append("<br /><br /><br />");
            body.Append("Se você tiver algum problema com o link acima, clique com o botão direito nele e copie a URL no navegador.");
            body.Append("<br /><br />");
            body.Append("E-mail enviado automaticamente, favor não responder");

            body.Append("</div>");

            await this._mailComponent.SendEmail(user.email, user.name, subject, body.ToString());
        }

        public async Task SendContactEmail(string title, string message, string userName, string cooperativeName, string to, List<string> cc)
        {
            var subject = $"[Contato Via Plataforma] {title}";
            var body = new StringBuilder();

            body.Append("<div style='font-size: 0.9rem;'>");

            body.Append($"<b>Usuário</b>: {userName}");
            body.Append("<br />");
            body.Append($"<b>Cooperativa</b>: {cooperativeName}");
            body.Append("<br /><br />");

            body.Append($"<b>Mensagem</b>: ");
            body.Append("<br />");
            body.Append(message);
            body.Append("<br /><br />");

            body.Append("</div>");

            await this._mailComponent.SendEmail(to, String.Empty, subject, body.ToString(), cc);
        }

        public async Task SendRecoverPasswordEmail(User user, string urlRecoverPassword)
        {
            var subject = "Redefina sua senha";
            var body = new StringBuilder();

            body.Append("<div style='font-size: 0.9rem;'>");

            body.Append($"Olá, {user.name.GetFirstWord()}!");
            body.Append("<br /><br /><br />");
            body.Append("Você solicitou a redefinição da sua senha!");
            body.Append("<br /><br />");
            body.Append($"<a href='{urlRecoverPassword}'>Clique aqui</a> para continuar com a redefinição.");
            body.Append("<br /><br /><br />");
            body.Append("Se você tiver algum problema com o link acima, clique com o botão direito nele e copie a URL no navegador.");
            body.Append("<br /><br />");
            body.Append("E-mail enviado automaticamente, favor não responder");

            body.Append("</div>");

            await this._mailComponent.SendEmail(user.email, user.name, subject, body.ToString());
        }
    }
}
