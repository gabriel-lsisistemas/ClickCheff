using Microsoft.AspNetCore.Mvc;
using ApiClickCheff.Model;
using ApiClickCheff.Repositorio;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity.Data;

namespace ApiClickCheff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MesaController : ControllerBase
    {
        private readonly RepositorioMesas _repositorioMesas;

        public MesaController()
        {
            _repositorioMesas = new RepositorioMesas();
        }

        [HttpGet("todas")]
        public ActionResult<ApiResponse<IEnumerable<Mesas>>> GetMesas()
        {
            try
            {
                var mesas = _repositorioMesas.GetMesas();
                var response = new ApiResponse<IEnumerable<Mesas>> { Data = mesas };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter as mesas: {ex.Message}");
            }
        }
        [HttpPost("atualizar-comanda")]
        public ActionResult<ApiResponse<string>> AtualizarComanda([FromBody] ComandaUpdateDto dto)
        {
            try
            {
                bool sucesso = _repositorioMesas.AtualizarComanda(dto);
                if (sucesso)
                    return Ok(new ApiResponse<string> { Data = "Comanda atualizada com sucesso" });

                return NotFound(new ApiResponse<string> { Data = "Comanda não encontrada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar comanda: {ex.Message}");
            }
        }
        [HttpGet("verifica")]
        public ActionResult<ApiResponse<IEnumerable<Mesas>>> VerificaMesa([FromQuery] int id)
        {
            try
            {
                var mesas = _repositorioMesas.VerificaMesa(id);

                if (mesas == null || !mesas.Any())
                    return NotFound(new ApiResponse<IEnumerable<Mesas>> { Data = null });

                return Ok(new ApiResponse<IEnumerable<Mesas>> { Data = mesas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao verificar mesa: {ex.Message}");
            }
        }

    }
}
