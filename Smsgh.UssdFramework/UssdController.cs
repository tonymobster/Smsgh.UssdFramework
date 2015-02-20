using System;
using System.Collections.Generic;

namespace Smsgh.UssdFramework
{
    public class UssdController : IDisposable
    {
        public UssdRequest Request { get; set; }
        public Dictionary<string, string> Data { get; set; } 
        public UssdDataBag DataBag { get; set; }

        #region Responders

        private string Route(string action, string controller = null)
        {
            if (controller == null)
            {
                controller = this.GetType().Name;
            }
            else
            {
                controller += "Controller";
            }
            return string.Format("{0}.{1}", controller, action);
        }

        public UssdResponse Redirect(string action, string controller = null)
        {
            return UssdResponse.Redirect(Route(action, controller));
        }


        public UssdResponse Render(string message, string action = null, 
            string controller = null)
        {
            string route = null;
            if (action != null)
            {
                route = Route(action, controller);
            }
            return UssdResponse.Render(message, route);
        }
        #endregion


        public virtual void Dispose()
        {
        }
    }
}
