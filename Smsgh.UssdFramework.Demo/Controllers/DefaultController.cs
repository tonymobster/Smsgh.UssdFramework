using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Smsgh.UssdFramework.Stores;

namespace Smsgh.UssdFramework.Demo.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Index(UssdRequest request)
        {
            var ussd = new Ussd();
            return Ok(await ussd.Process(new RedisStore(), request, "Main", "Menu"));
        } 
    }
}
