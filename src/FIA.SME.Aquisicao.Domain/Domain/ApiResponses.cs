namespace FIA.SME.Aquisicao.Core.Domain
{
    /// <summary>
    /// Response padrão da API para o erro HTTP 204
    /// </summary>
    public class NoContentApiResponse : Saida
    {
        public NoContentApiResponse() : this("Informação não encontrada.") { }

        public NoContentApiResponse(string message)
        {
            this.Sucesso = false;
            this.Mensagens = new[] { message };
            this.Retorno = null;
            this.StatusCode = 204;
        }

        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = false,
                Mensagens = new[] { "Informação não encontrada." },
                Retorno = null,
                StatusCode = 204
            };
        }
    }


    /// <summary>
    /// Response padrão da API para o erro HTTP 401
    /// </summary>
    public class UnauthorizedApiResponse : Saida
    {
        public UnauthorizedApiResponse()
        {
            this.Sucesso = false;
            this.Mensagens = new[] { "Acesso negado" };
            this.Retorno = null;
            this.StatusCode = 401;
        }

        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = false,
                Mensagens = new[] { "Erro 401: Acesso negado. Certifique-se que você foi autenticado." },
                Retorno = null,
                StatusCode = 401
            };
        }
    }

    /// <summary>
    /// Response padrão da API para o erro HTTP 403
    /// </summary>
    public class ForbiddenApiResponse : Saida
    {
        public ForbiddenApiResponse()
        {
            this.Sucesso = false;
            this.Mensagens = new[] { "Erro 403: Acesso negado. Você não tem permissão de acesso para essa funcionalidade." };
            this.Retorno = null;
            this.StatusCode = 403;
        }

        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = false,
                Mensagens = new[] { "Erro 403: Acesso negado. Você não tem permissão de acesso para essa funcionalidade." },
                Retorno = null,
                StatusCode = 403
            };
        }
    }

    /// <summary>
    /// Response padrão da API para o erro HTTP 404
    /// </summary>
    public class NotFoundApiResponse : Saida
    {
        public NotFoundApiResponse()
        {
            this.Sucesso = false;
            this.Mensagens = new[] { "Erro 404: O endereço não encontrado." };
            this.Retorno = null;
            this.StatusCode = 404;
        }

        public NotFoundApiResponse(string path)
        {
            this.Sucesso = false;
            this.Mensagens = new[] { $"Erro 404: O endereço \"{path}\" não foi encontrado." };
            this.Retorno = null;
            this.StatusCode = 404;
        }

        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = false,
                Mensagens = new[] { "Erro 404: O endereço não encontrado." },
                Retorno = null,
                StatusCode = 404
            };
        }
    }

    /// <summary>
    /// Response padrão da API para o erro HTTP 415
    /// </summary>
    public class UnsupportedMediaTypeApiResponse : Saida
    {
        public UnsupportedMediaTypeApiResponse()
        {
            this.Sucesso = false;
            this.Mensagens = new[] { "Erro 415: O tipo de requisição  não é suportado pela API." };
            this.Retorno = null;
            this.StatusCode = 415;
        }

        public UnsupportedMediaTypeApiResponse(string requestContentType)
        {
            this.Sucesso = false;
            this.Mensagens = new[] { $"Erro 415: O tipo de requisição \"{requestContentType}\" não é suportado pela API." };
            this.Retorno = null;
            this.StatusCode = 415;
        }

        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = false,
                Mensagens = new[] { "Erro 415: O tipo de requisição não é suportado pela API." },
                Retorno = null,
                StatusCode = 415
            };
        }
    }

    /// <summary>
    /// Response padrão da API para o erro HTTP 400
    /// </summary>
    public class BadRequestApiResponse : Saida
    {
        public BadRequestApiResponse(object? retorno) : this("Ocorreu um erro ao tentar executar esta ação", retorno) { }

        public BadRequestApiResponse(string mensagem, object? retorno = null)
        {
            this.Sucesso = false;
            this.Mensagens = new[] { mensagem };
            this.Retorno = retorno;
            this.StatusCode = 400;
        }

        public Saida GetExamples()
        {
            return new Saida
            {
                Sucesso = false,
                Mensagens = new[] { "Ocorreu um erro ao tentar executar esta ação" },
                Retorno = null,
                StatusCode = 400
            };
        }
    }

    /// <summary>
    /// Response padrão da API para o erro HTTP 500
    /// </summary>
    public class InternalServerErrorApiResponse : Saida
    {
        public InternalServerErrorApiResponse()
        {
            this.Sucesso = false;
            this.Mensagens = new[] { "Ocorreu um erro inesperado." };
            this.Retorno = null;
            this.StatusCode = 500;
        }

        public InternalServerErrorApiResponse(Exception exception)
        {
            if (exception == null)
                return;

            this.Sucesso = false;
            this.Mensagens = new[] { exception.GetBaseException().Message };
            this.Retorno = new
            {
                Exception = exception.Message,
                BaseException = exception.GetBaseException().Message,
                exception.Source
            };
            this.StatusCode = 500;
        }

        public Saida? GetExamples()
        {
            try
            {
                var i = 1;

                try
                {
                    i /= 0;
                }
                catch (Exception ex1)
                {
                    throw new Exception("Ocorreu um erro inesperado.", ex1);
                }

                return null;
            }
            catch (Exception ex2)
            {
                return new InternalServerErrorApiResponse(ex2);
            }
        }
    }
}
