using System;
using System.Threading.Tasks;

namespace Smsgh.UssdFramework.Demo.UssdActions
{
    public static class Menus
    {
        public static async Task<string> Main(UssdContext context)
        {
            return "Welcome" + Environment.NewLine
                   + "1. Say my name" + Environment.NewLine
                   + "2. Exit";
        }

        public static async Task<string> SayMyName(UssdContext context)
        {
            return "Which name do you want to display?" + Environment.NewLine
                   + "1. Real name" + Environment.NewLine
                   + "2. Nickname";
        } 
    }
}