using System.Web.Http;

namespace MathCalcWebApp.Controller
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("status")]
        public IHttpActionResult Status()
        {
            return this.Ok("All fine!");
        }
    }
}
