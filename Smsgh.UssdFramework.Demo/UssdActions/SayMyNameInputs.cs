using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Smsgh.UssdFramework.Demo.UssdActions
{
    public static class SayMyNameInputs
    {
        public static async Task<UssdResponse> RealName(UssdContext context)
        {
            return context.Response("Enter first name:"
                , "SayMyName/RealNameFirstName");
        }

        public async static Task<UssdResponse> RealNameFirstName(UssdContext context)
        {
            await context.DataBag.Set("FirstName", context.Request.SanitizedMessage);
            return context.Response("Enter last name:",
                "SayMyName/RealNameLastName");
        }

        public async static Task<UssdResponse> RealNameLastName(UssdContext context)
        {
            var first = await context.DataBag.Get("FirstName");
            var last = context.Request.SanitizedMessage;
            return context.Response(string.Format("Have a great day, {0} {1}",
                first, last));
        }
    }
}