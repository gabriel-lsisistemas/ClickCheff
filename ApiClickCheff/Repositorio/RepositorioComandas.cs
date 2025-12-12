using System.Collections.Generic;
using ApiClickCheff.Dao;
using ApiClickCheff.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiClickCheff.Repositorio
{
    public class RepositorioComandas
    {
        private readonly DaoComandas _daoComandas;

        public RepositorioComandas()
        {
            _daoComandas = new DaoComandas();
        }

        public List<Comandas> GetComandasMesa(int id)
        {
            return _daoComandas.GetComandasMesa(id);
        }
        public void InserirComanda(ComandaInsert comanda)
        {
            _daoComandas.InserirComanda(comanda);
        }
        public void InserirOuAtualizarComandaDetalhe(ComandaDetalheInsert detalhe)
        {
            _daoComandas.InserirOuAtualizarComandaDetalhe(detalhe);
        }
        public void InserirOuAtualizarComandaDetalheSabores(int idProdutoSabor, int idProduto, int idComanda)
        {
            _daoComandas.InserirOuAtualizarComandaDetalheSabores(idProdutoSabor, idProduto, idComanda);
        }
        public void AtualizarImpressaoComandaDetalhe(int idComandaDetalhe)
        {
            _daoComandas.AtualizarImpressaoComandaDetalhe(idComandaDetalhe);
        }
        public List<UltimoIDComanda> GetUltimoID()
        {
            return _daoComandas.GetUltimoID();
        }

    }
}
