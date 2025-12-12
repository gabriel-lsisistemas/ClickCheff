using Microsoft.AspNetCore.Mvc;
using ApiClickCheff.Model;
using ApiClickCheff.Repositorio;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.Data;

namespace ApiClickCheff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComandasController : ControllerBase
    {
        private readonly RepositorioComandas _repositorioComandas;

        public ComandasController()
        {
            _repositorioComandas = new RepositorioComandas();
        }

        [HttpGet("PorMesa")]
        public ActionResult<ApiResponse<IEnumerable<Comandas>>> GetComandasMesa([FromQuery] int id)
        {
            try
            {
                var comandas = _repositorioComandas.GetComandasMesa(id);

                if (comandas == null || !comandas.Any())
                    return NotFound(new ApiResponse<IEnumerable<Comandas>> { Data = null });

                return Ok(new ApiResponse<IEnumerable<Comandas>> { Data = comandas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao verificar comandas: {ex.Message}");
            }
        }

        [HttpPost("inserir")]
        public IActionResult InserirComanda([FromBody] ComandaInsert comanda)
        {
            try
            {
                _repositorioComandas.InserirComanda(comanda);
                return Ok(new { sucesso = true, mensagem = "Comanda inserida com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = $"Erro ao inserir comanda: {ex.Message}" });
            }
        }
        [HttpPost("comanda-detalhe")]
        public IActionResult InserirOuAtualizarComandaDetalhe([FromBody] ComandaDetalheInsert detalhe)
        {
            try
            {
                _repositorioComandas.InserirOuAtualizarComandaDetalhe(detalhe);
                return Ok(new { sucesso = true, mensagem = "Comanda detalhe processada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }
        [HttpGet("ultimoID")]
        public ActionResult<ApiResponse<IEnumerable<UltimoIDComanda>>> GetUltimoID()
        {
            try
            {
                var ultimoid = _repositorioComandas.GetUltimoID();
                var response = new ApiResponse<IEnumerable<UltimoIDComanda>> { Data = ultimoid };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter as comandas: {ex.Message}");
            }
        }
        [HttpGet("DetalheSabor")]
        public ActionResult InserirOuAtualizarComandaDetalheSabores(
            [FromQuery] int idProdutoSabor,
            [FromQuery] int idProduto,
            [FromQuery] int idComanda)
        {
            try
            {
                _repositorioComandas.InserirOuAtualizarComandaDetalheSabores(idProdutoSabor, idProduto, idComanda);
                return Ok(new { sucesso = true, mensagem = "Registro inserido ou atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = $"Erro ao inserir/atualizar detalhe de sabor: {ex.Message}" });
            }
        }
        [HttpGet("AtualizaIMP")]
        public ActionResult AtualizarImpressaoComandaDetalhe([FromQuery] int idComandaDetalhe)
        {
            try
            {
                _repositorioComandas.AtualizarImpressaoComandaDetalhe(idComandaDetalhe);
                return Ok(new { sucesso = true, mensagem = "Registro atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = $"Erro ao atualizar detalhe de sabor: {ex.Message}" });
            }
        }
    }
}
