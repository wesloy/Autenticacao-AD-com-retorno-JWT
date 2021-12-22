using System;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SI_MicroServicos.Model;
using SI_MicroServicos.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;

namespace SI_MicroServicos.Controllers
{
    /// <summary>
    /// Auth tem como objetivo ser um serviço de retorno de token
    /// a partir de credencias do AD (Active Directory) válidas.
    /// Os parâmetros necessários para obter um token é:
    /// - Usuário de rede Algar
    /// - Senha, que não é salva dentro deste serviço de autenticação.
    /// - Sistema que está solicitando a autenticação
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly DbContext _DbContext;


        private IConfiguration _config;
        /// <summary>
        /// Auth controller
        /// </summary>
        public AuthController(IConfiguration Configuration)
        {
            _config = Configuration;

        }

        /// <summary>
        /// Autenticar usuário no sistema, utilizando credencias do AD.
        /// </summary>
        /// <param name="detailsLogin"></param>
        /// <returns>
        /// Este método log todas as solicitações de autenticação feitas a ele.
        /// </returns>
        [HttpPost, AllowAnonymous]
        [ProducesResponseType(typeof(UserAuth), StatusCodes.Status200OK)] //Retorno Ok no Swagger
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)] //Retorno Error de Servidor no Swagger
        [EnableCors("CorsPolicy")]
        public IActionResult Auth([FromBody] UserAuth detailsLogin)
        {

            UserProfile result = ValidateUser(detailsLogin);
            if (result != null)
            {
                var role = result.Type;
                var tokenString = generateTokenJWT(result);

                try
                {   
                    result.ID = Guid.Empty;
                    result.Password = null;
                    return Ok(new { token = tokenString, user = result });
                }

                catch (Exception ex)
                {
                    return Unauthorized(ex);
                }
            }

            else
            {
                return Unauthorized("Erro credenciais");
            }



        }


        private string generateTokenJWT(UserProfile user)
        {
            var role = user.Type.ToString();
            var id = user.ID.ToString();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Type.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        private UserProfile ValidateUser(UserAuth detailsLogin)
        {
            var validateUser = new UserServices().GetAndValidate(detailsLogin);
            if (validateUser != null)
            {

                return validateUser;
            }
            return null;

        }


    }
}
