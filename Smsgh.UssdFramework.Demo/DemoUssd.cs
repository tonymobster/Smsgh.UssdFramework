using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Smsgh.UssdFramework.Demo.UssdActions.Menus;
using Smsgh.UssdFramework.Stores;

namespace Smsgh.UssdFramework.Demo
{
    public class DemoUssd : Ussd
    {
        public DemoUssd() : base(new RedisStore())
        {
        }

        public void Routes()
        {
            RouteMenu("main-menu", Default.MainMenu, new Dictionary<string, string>()
            {
                {"1", "say-my-name"},
            });
        }
    }
}