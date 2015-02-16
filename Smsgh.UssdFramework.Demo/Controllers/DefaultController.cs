using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Smsgh.UssdFramework.Demo.UssdActions.Menus;

namespace Smsgh.UssdFramework.Demo.Controllers
{
    public class DefaultController : ApiController
    {
        private Ussd Ussd { get; set; }

        public DefaultController()
        {
            var ussd = new DemoUssd();
            ussd.Routes();
            Ussd = ussd;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Index(UssdRequest request)
        {
            return Ok(await Ussd.Process(request, "main-menu"));
        } 
    }
}
