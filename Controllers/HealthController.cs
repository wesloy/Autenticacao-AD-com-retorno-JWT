using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace sheduler_backend.Controllers
{
    public class HealthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        #region Health API
        /// <summary>
        /// Verifica status da API
        /// </summary>
        /// <remarks>
        /// Metodo para verificar o status da API
        /// Autenticação não obrigatória
        /// </remarks>
        /// <returns>Retornará "Hello API!", caso o esteja tudo OK</returns>
        [AllowAnonymous]
        [HttpGet("/api/[controller]")]
        public IActionResult Health()
        {
            return Ok("Hello API!");
        }

        #endregion
    }
}