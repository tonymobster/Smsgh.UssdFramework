using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Smsgh.UssdFramework.Demo.Controllers
{
    public class MainController : UssdController
    {
        public async Task<UssdResponse> Menu()
        {
            return Render("Welcome to say my name" + Environment.NewLine
                          + "1. Full name",
                "MenuProcessor");
        }

        public async Task<UssdResponse> MenuProcessor()
        {
            return Request.SanitizedMessage == "1" 
                ? Render("Enter full  name", "FullName") 
                : Render("No choice selected.");
        }

        public async Task<UssdResponse> FullName()
        {
            return Render(Request.SanitizedMessage);
        } 
    }
}