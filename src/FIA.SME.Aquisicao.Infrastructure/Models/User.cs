using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class User
    {
        #region [ Construtores ]

        public User(string name, string email, string password, string role = "", bool? isActive = null)
        {
            this.name = name.ToTitleCase();
            this.email = email.ToLower();
            this.password = password;
            this.role = String.IsNullOrEmpty(role) ? this.role : role;
            this.is_active = isActive ?? true;
        }

        internal User(Usuario? usuario)
        {
            if (usuario == null)
                return;

            this.id = usuario.id;
            this.email = usuario.email;
            this.name = usuario.nome;
            this.password = usuario.senha;
            this.role = usuario.perfil;
            this.is_active = usuario.ativo;
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id          { get; private set; }
        public string name      { get; private set; } = String.Empty;
        public string email     { get; private set; } = String.Empty;
        public string password  { get; private set; } = String.Empty;
        public string role      { get; private set; } = String.Empty;
        public bool is_active   { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void HashPassword(IPasswordHasher passwordHasher, string password)
        {
            var hashedPassword = passwordHasher.HashPassword(this.id.ToString(), password);
            this.password = hashedPassword;
        }

        public void Disable()
        {
            this.is_active = false;
        }

        public void SetId(Guid id)
        {
            this.id = id;
        }

        public void SetRole(RoleEnum roleEnum)
        {
            this.role = roleEnum.ToString().ToLower();
        }

        #endregion [ FIM - Metodos ]
    }
}
