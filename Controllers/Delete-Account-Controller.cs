using AppVidaSana.ProducesResponseType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("App - Account_Profile")]
    [ApiExplorerSettings(GroupName = "app")]
    [Route("delete-account")]
    [RequestTimeout("CustomPolicy")]
    public class DeleteAccountController : Controller
    {
        /// <summary>
        /// This is the driver for the delete account view.
        /// </summary>
        /// <response code="404">Returns a message indicating that the page could not be loaded correctly.</response>
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DeleteAccountPatient()
        {
            try
            {
                return View("~/Views/DeleteAccountPage/DeleteAccountPatient.cshtml");
            }
            catch (Exception)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    message = "Hubo un error, inténtelo de nuevo.",
                    status = "No se cargo completamente la página"
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });

            }
        }
    }
}
