using System.Collections.Generic;
using ApiClickCheff.Dao;
using ApiClickCheff.Model;

namespace ApiClickCheff.Repositorio
{
    public class RepositorioMesas
    {
        private readonly DaoMesa _daoMesa;

        public RepositorioMesas()
        {
            _daoMesa = new DaoMesa();
        }

        public List<Mesas> GetMesas()
        {
            return _daoMesa.GetMesas();
        }
        public bool AtualizarComanda(ComandaUpdateDto dto)
        {
            return _daoMesa.AtualizarComanda(dto);
        }
        public List<Mesas> VerificaMesa(int id)
        {
            return _daoMesa.VerificaMesa(id);
        }

    }
}
