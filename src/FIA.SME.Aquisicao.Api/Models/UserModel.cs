using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class UserLogin
    {
        public string email     { get; set; } = String.Empty;
        public string password  { get; set; } = String.Empty;
    }

    public class UserEmail
    {
        public string email     { get; set; } = String.Empty;
    }

    public class UserChangePassword
    {
        public string email     { get; set; } = String.Empty;
        public string password  { get; set; } = String.Empty;
    }

    public class UserRecoverPassword
    {
        public string email     { get; set; } = String.Empty;
        public string password  { get; set; } = String.Empty;
        public string token     { get; set; } = String.Empty;
    }

    public class UserRegister
    {
        public string name      { get; set; } = String.Empty;
        public string email     { get; set; } = String.Empty;
        public string password  { get; set; } = String.Empty;
        public string role      { get; set; } = String.Empty;
    }

    public class UserUpdate
    {
        public Guid id          { get; set; }
        public string name      { get; set; } = String.Empty;
        public string email     { get; set; } = String.Empty;
        public bool is_active   { get; set; } = true;
    }

    public class UserLoginResponse
    {
        public UserLoginResponse(User user, string token)
        {
            this.id = user.id;
            this.email = user.email;
            this.name = user.name;
            this.token = token;
            this.roles = user.role?.Split(',')?.ToList() ?? new List<string>();
        }

        public Guid id              { get; set; }
        public string email         { get; set; }
        public string name          { get; set; }
        public string firstName     { get { return this.name.GetFirstWord(); } }
        public string token         { get; set; }
        public List<string> roles   { get; set; }
    }

    public class UserResponse
    {
        public UserResponse(User? user)
        {
            if (user == null)
                return;

            this.id = user.id;
            this.email = user.email;
            this.name = user.name;
            this.roles = user.role?.Split(',')?.ToList() ?? new List<string>();
            this.is_active = user.is_active;
        }

        public Guid id              { get; set; }
        public string email         { get; set; }
        public string name          { get; set; }
        public string firstName     { get { return this.name.GetFirstWord(); } }
        public List<string> roles   { get; set; }
        public bool is_active       { get; set; }
    }
}
