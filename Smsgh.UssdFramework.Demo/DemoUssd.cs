using System.Collections.Generic;
using Smsgh.UssdFramework.Demo.UssdActions;
using Smsgh.UssdFramework.Stores;

namespace Smsgh.UssdFramework.Demo
{
    public class DemoUssd : UssdFramework.Ussd
    {
        public DemoUssd() : base(new RedisStore())
        {
        }

        public void Routes()
        {
            RouteMenu("main-menu", Menus.Main, new Dictionary<string, string>()
            {
                {"1", "say-my-name"},
            });

            RouteMenu("say-my-name", Menus.SayMyName, new Dictionary<string, string>()
            {
                {"1", "say-my-name/real-name"},
            });
        }
    }
}