using Microsoft.AspNetCore.Mvc;
using ApiClickCheff.Model;
using ApiClickCheff.Repositorio;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity.Data;

namespace ApiClickCheff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly RepositorioProduto _repositorioProduto;

        public ProdutoController()
        {
            _repositorioProduto = new RepositorioProduto();
        }

        [HttpGet("GrupoProduto")]
        public ActionResult<ApiResponse<IEnumerable<GrupoProduto>>> GetGrupoProduto()
        {
            try
            {
                var grupos = _repositorioProduto.GetGrupoProdutos();
                var response = new ApiResponse<IEnumerable<GrupoProduto>> { Data = grupos };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter os grupos: {ex.Message}");
            }
        }
        [HttpGet("ProdutoComGrupo")]
        public ActionResult<ApiResponse<IEnumerable<Produto>>> VerificaMesa([FromQuery] int id)
        {
            try
            {
                var produtosComGrupos = _repositorioProduto.GetProdutosComGrupo(id);

                if (produtosComGrupos == null || !produtosComGrupos.Any())
                    return NotFound(new ApiResponse<IEnumerable<Produto>> { Data = null });

                return Ok(new ApiResponse<IEnumerable<Produto>> { Data = produtosComGrupos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao verificar mesa: {ex.Message}");
            }
        }
        [HttpGet("Sabores")]
        public ActionResult<ApiResponse<IEnumerable<Sabores>>> GetSabores([FromQuery] int idProduto)
        {
            try
            {
                var sabores = _repositorioProduto.GetSabores(idProduto);

                if (sabores == null || !sabores.Any())
                    return NotFound(new ApiResponse<IEnumerable<Sabores>> { Data = null });

                return Ok(new ApiResponse<IEnumerable<Sabores>> { Data = sabores });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro sabores: {ex.Message}");
            }
        }
        [HttpGet("Produto")]
        public ActionResult<ApiResponse<IEnumerable<Produto>>> GetProduto()
        {
            try
            {
                var produtos = _repositorioProduto.GetProdutos();
                var response = new ApiResponse<IEnumerable<Produto>> { Data = produtos };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter os grupos: {ex.Message}");
            }
        }
        [HttpGet("MaisVendidos")]
        public ActionResult<ApiResponse<IEnumerable<Produto>>> GetMaisVendidos()
        {
            try
            {
                var maisvendidos = _repositorioProduto.GetMaisVendidos();
                var response = new ApiResponse<IEnumerable<Produto>> { Data = maisvendidos };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter os grupos: {ex.Message}");
            }
        }
    }
}
