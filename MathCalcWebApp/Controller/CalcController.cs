using System.Web.Http;
using org.mariuszgromada.math.mxparser;

namespace MathCalcWebApp.Controller
{
    public class CalcController : ApiController
    {
        [HttpGet]
        [Route("calc")]
        public IHttpActionResult CalcViaGet(string eq)
        {
            Expression e = new Expression(eq); // adding a comment

            return this.Ok(e.calculate());
        }
    }
}
