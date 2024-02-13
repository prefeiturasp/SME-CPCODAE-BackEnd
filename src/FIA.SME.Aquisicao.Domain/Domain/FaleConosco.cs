namespace FIA.SME.Aquisicao.Core.Domain
{
    public class FaleConosco
    {
        public FaleConosco(string title, string message, string userName, string cooperativeName, string userEmail, string cooperativeEmail)
        {
            Title = title;
            Message = message;
            UserName = userName;
            CooperativeName = cooperativeName;
            UserEmail = userEmail;
            CooperativeEmail = cooperativeEmail;
        }

        public string Title { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public string UserName { get; private set; } = null!;
        public string CooperativeName { get; private set; } = null!;
        public string UserEmail { get; private set; } = null!;
        public string CooperativeEmail { get; private set; } = null!;
    }
}
