using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smsgh.UssdFramework
{
    public class UssdView
    {
        public string Message { get; set; }
        public bool IsRelease { get; set; }

        public UssdView(string message, bool isRelease)
        {
            Message = message;
            IsRelease = isRelease;
        }

        public static UssdView Response(string message)
        {
            return new UssdView(message, false);
        }

        public static UssdView Release(string message)
        {
            return new UssdView(message, true);
        }
    }
}
