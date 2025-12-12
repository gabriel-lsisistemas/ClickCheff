using System.Collections.Generic;
using ApiClickCheff.Dao;
using ApiClickCheff.Model;

namespace ApiClickCheff.Repositorio
{
    public class RepositorioLogin
    {
        private readonly DaoLogin _daoLogin;

        public RepositorioLogin()
        {
            _daoLogin= new DaoLogin();
        }

        public List<Login> GetLogins()
        {
            return _daoLogin.GetLogins();
        }

        public Login GetLoginByCredentials(string login, string senhaEnviada)
        {
            return _daoLogin.GetLoginByCredentials(login, senhaEnviada);
        }

    }
}
