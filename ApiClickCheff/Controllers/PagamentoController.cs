using Microsoft.AspNetCore.Mvc;
using ApiClickCheff.Model;
using ApiClickCheff.Repositorio;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity.Data;

namespace ApiClickCheff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly RepositorioPagamento _repositorioPagamento;

        public PagamentoController()
        {
            _repositorioPagamento = new RepositorioPagamento();
        }

        [HttpGet("TipoPagamento")]
        public ActionResult<ApiResponse<IEnumerable<FormaPagamento>>> GetTipoPagamento()
        {
            try
            {
                var tipos = _repositorioPagamento.GetTipoPagamento();
                var response = new ApiResponse<IEnumerable<FormaPagamento>> { Data = tipos };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter os pagamentos: {ex.Message}");
            }
        }
        [HttpPost("InserirComandaPagamento")]
        public ActionResult InserirComandaPagamento([FromBody] ComandaPagamentoRequest request)
        {
            try
            {
                var sucesso = _repositorioPagamento.InserirComandaPagamento(
                    request.ID_ECF_TIPO_PAGAMENTO,
                    request.ID_COMANDA,
                    request.VALOR
                );

                if (sucesso)
                    return Ok(new { message = "Pagamento inserido com sucesso." });
                else
                    return StatusCode(500, "Falha ao inserir o pagamento.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao inserir pagamento: {ex.Message}");
            }
        }

    }
}
