using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smsgh.UssdFramework
{
    public class UssdResponse
    {
        public string Type { get; private set; }
        public string Message { get; private set; }
        public string ClientState { get; private set; }


        public string NextRoute { get; private set; }
        public bool Release { get { return string.IsNullOrWhiteSpace(NextRoute); } }

        public static UssdResponse New(string message, string nextRoute = null)
        {
            var type = string.IsNullOrWhiteSpace(nextRoute)
                ? UssdResponseTypes.Release.ToString()
                : UssdResponseTypes.Response.ToString();
            return new UssdResponse()
            {
                Type = type,
                Message = message,
            };
        }

        private enum UssdResponseTypes
        {
            Response, Release,
        }
    }
}
