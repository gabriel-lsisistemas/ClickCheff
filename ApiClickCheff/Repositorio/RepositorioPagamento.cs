using System.Collections.Generic;
using ApiClickCheff.Dao;
using ApiClickCheff.Model;

namespace ApiClickCheff.Repositorio
{
    public class RepositorioPagamento
    {
        private readonly DaoPagamentos _daoPagamentos;

        public RepositorioPagamento()
        {
            _daoPagamentos = new DaoPagamentos();
        }

        public List<FormaPagamento> GetTipoPagamento()
        {
            return _daoPagamentos.GetTipoPagamento();
        }
        public bool InserirComandaPagamento(int idTipoPagamento, int idComanda, decimal valor)
        {
            return _daoPagamentos.InserirComandaPagamento(idTipoPagamento, idComanda, valor);
        }

    }
}
