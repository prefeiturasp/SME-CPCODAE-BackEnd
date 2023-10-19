using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class Bank
    {
        public Bank() { }

        public Bank(Guid id, string code, string name, string agency, string account_number)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.agency = agency;
            this.account_number = account_number;
        }

        internal Bank(Banco banco)
        {
            if (banco == null)
                return;

            this.id = banco.id;
            this.code = banco.codigo;
            this.name = banco.nome;
            this.agency = banco.agencia;
            this.account_number = banco.numero_conta;
        }

        public Guid id                  { get; private set; }
        public string code              { get; private set; }
        public string name              { get; private set; }
        public string agency            { get; private set; }
        public string account_number    { get; private set; }
    }
}
