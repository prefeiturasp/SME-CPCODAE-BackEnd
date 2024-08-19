namespace FIA.SME.Aquisicao.Core.Domain
{
    public class FaleConosco
    {
        public FaleConosco(string title, string message, string userName, string cooperativeName, string userEmail, string cooperativeEmail, string publicCallName, string publicCallNumber, string publicCallProcess)
        {
            Title = title;
            Message = message;
            UserName = userName;
            CooperativeName = cooperativeName;
            UserEmail = userEmail;
            CooperativeEmail = cooperativeEmail;
            PublicCallName = publicCallName;
            PublicCallNumber = publicCallNumber;
            PublicCallProcess = publicCallProcess;
        }

        public string Title { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public string UserName { get; private set; } = null!;
        public string CooperativeName { get; private set; } = null!;
        public string UserEmail { get; private set; } = null!;
        public string CooperativeEmail { get; private set; } = null!;
        public string PublicCallName { get; private set; } = null!;
        public string PublicCallNumber { get; private set; } = null!;
        public string PublicCallProcess { get; private set; } = null!;
    }
}
