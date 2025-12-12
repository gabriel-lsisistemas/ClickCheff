using Microsoft.AspNetCore.Mvc;
using ApiClickCheff.Model;
using ApiClickCheff.Repositorio;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity.Data;

namespace ApiClickCheff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly RepositorioLogin _repositorioLogin;

        public LoginController()
        {
            _repositorioLogin = new RepositorioLogin();
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<Login>>> GetLogin()
        {
            try
            {
                var logins = _repositorioLogin.GetLogins();
                var response = new ApiResponse<IEnumerable<Login>> { Data = logins };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter os tipos de pagamento: {ex.Message}");
            }
        }
        [HttpPost]
        public ActionResult<ApiResponse<Login>> PostLogin([FromBody] Model.LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Senha))
                {
                    return BadRequest(new ApiResponse<Login> { Message = "Login e senha são obrigatórios." });
                }

                var login = _repositorioLogin.GetLoginByCredentials(request.Login, request.Senha);

                if (login == null)
                {
                    return NotFound(new ApiResponse<Login> { Message = "Login ou senha inválidos." });
                }

                var response = new ApiResponse<Login> { Data = login };
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<Login> { Message = "Senha incorreta." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Login> { Message = $"Erro ao realizar o login: {ex.Message}" });
            }
        }
    }
}
