using CorreoFei.Models;
using CorreoFei.Services.Email;
using CorreoFei.Services.ErrorLog;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace CorreoFei.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmail _email;
        private readonly IErrorLog _errorlog;

        public HomeController(IErrorLog errorlog, IEmail email)
        {
            _errorlog = errorlog;
            _email = email;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAsync(ContactoViewModel contacto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Envía correo
                    await _email.EnviaCorreoAsync("Correo electrónico desde FEI", contacto.Correo, null, null, CuerpoCorreo(contacto.Nombre));

                    return RedirectToAction(nameof(Success));
                }
                catch (Exception ex)
                {
                    await _errorlog.ErrorLogAsync(ex.Message);
                }
            }

            ModelState.AddModelError("", "No ha sido posible enviar el correo. Inténtelo nuevamente.");
            return View();
        }

        public string CuerpoCorreo(string nombre)
        {
            string Mensaje = $"<p>Estimado/Estimada usuario, {nombre}</p>";
            Mensaje += "<p>Por este medio le informamos que su participación ha sido recibida.</p>";

            Mensaje += "<p>Agradecemos su participación.</p>";
            Mensaje += "<br /><br /><br /><div>-----------------------------------------------------------------</div>";
            Mensaje += "<div>Mensaje enviado automáticamente. Favor de no responder al remitente de este mensaje.</div>";

            return Mensaje;
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}