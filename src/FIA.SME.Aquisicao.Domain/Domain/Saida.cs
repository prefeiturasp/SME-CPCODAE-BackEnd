namespace FIA.SME.Aquisicao.Core.Domain
{
    /// <summary>
    /// Comando para padronização das saídas do domínio
    /// </summary>
    public class Saida
    {
        public Saida() { }

        public Saida(int statusCode, bool sucesso, string mensagem, object? retorno) : this(statusCode, sucesso, new List<string>() { mensagem }, retorno) { }

        public Saida(int statusCode, bool sucesso, IEnumerable<string> mensagens, object? retorno)
        {
            this.Sucesso = sucesso;
            this.Mensagens = mensagens;
            this.Retorno = retorno;
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Indica se houve sucesso (nenhuma ocorrência de erro)
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensagens retornadas
        /// </summary>
        public IEnumerable<string> Mensagens { get; set; } = new List<string>();

        /// <summary>
        /// Objeto retornado
        /// </summary>
        public object? Retorno { get; set; }

        public int StatusCode { get; set; }
    }
}
