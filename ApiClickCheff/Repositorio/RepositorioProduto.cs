using System.Collections.Generic;
using ApiClickCheff.Dao;
using ApiClickCheff.Model;

namespace ApiClickCheff.Repositorio
{
    public class RepositorioProduto
    {
        private readonly DaoProduto _daoProduto;

        public RepositorioProduto()
        {
            _daoProduto = new DaoProduto();
        }

        public List<GrupoProduto> GetGrupoProdutos()
        {
            return _daoProduto.GetGrupoProdutos();
        }

        public List<Produto> GetProdutosComGrupo(int id)
        {
            return _daoProduto.GetProdutosComGrupo(id);
        }
        public List<Sabores> GetSabores(int idProduto)
        {
            return _daoProduto.GetSabores(idProduto);
        }
        public List<Produto> GetProdutos()
        {
            return _daoProduto.GetProdutos();
        }
        public List<Produto> GetMaisVendidos()
        {
            return _daoProduto.GetMaisVendidos();
        }
    }
}
