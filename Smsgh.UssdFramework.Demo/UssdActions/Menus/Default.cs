using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Smsgh.UssdFramework.Demo.UssdActions.Menus
{
    public static class Default
    {
        public static async Task<string> MainMenu(UssdContext context)
        {
            return string.Format("Welcome" + Environment.NewLine
                                 + "1. Say my name" + Environment.NewLine
                                 + "2. Exit");
        }
    }
}