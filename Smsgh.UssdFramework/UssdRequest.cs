using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smsgh.UssdFramework
{
    public class UssdRequest
    {
        public string Mobile { get; set; }
        public string SessionId { get; set; }
        public string ServiceCode { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Operator { get; set; }

        public UssdRequestTypes RequestType 
        {
            get
            {
                switch (Type.ToLower())
                {
                    case "initiation":
                        return UssdRequestTypes.Initiation;
                    case "response":
                        return UssdRequestTypes.Response;
                    case "release":
                        return UssdRequestTypes.Release;
                    default:
                        return UssdRequestTypes.Timeout;
                }
            }
        }

        public string SanitizedMessage
        {
            get { return Message.Trim(); }
        }
    }

    public enum UssdRequestTypes
    {
        Initiation,
        Response,
        Release,
        Timeout,
    }
}
