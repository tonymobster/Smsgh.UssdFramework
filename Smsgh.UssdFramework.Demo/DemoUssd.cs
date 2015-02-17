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
            RouteMenu("Start", Menus.Main, new Dictionary<string, string>()
            {
                {"1", "SayMyName"},
            });

            RouteMenu("SayMyName", Menus.SayMyName, new Dictionary<string, string>()
            {
                {"1", "SayMyName/RealName"},
                {"2", "SayMyName/Nickname"},
            });

            Route("SayMyName/RealName", SayMyNameInputs.RealName);
            Route("SayMyName/RealNameFirstName", SayMyNameInputs.RealNameFirstName);
            Route("SayMyName/RealNameLastName", SayMyNameInputs.RealNameLastName);
        }
    }
}