using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Smsgh.UssdFramework.Stores;

namespace Smsgh.UssdFramework
{
    public class UssdContext
    {
        private string NextRouteKey { get { return Request.Mobile + "NextRoute"; } }
        private string DataBagKey { get { return Request.Mobile + "DataBag"; } }
        private Dictionary<string, Func<UssdContext, Task<UssdResponse>>> Routes { get; set; }
        private Func<UssdContext, Task<UssdResponse>> Action { get; set; }
        public UssdRequest Request { get; private set; }
        private IStore Store { get; set; }
        public UssdDataBag DataBag { get; private set; }

        public UssdContext(Ussd ussd, UssdRequest request)
        {
            Store = ussd.Store;
            Routes = ussd.Routes;
            Request = request;
            DataBag = new UssdDataBag(Store, DataBagKey);
        }

        #region Responders
        /// <summary>
        /// Reroute context processing.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public async Task<UssdResponse> ReRoute(string route)
        {
            await SessionSetNextRoute(route);
            await SessionSetAction();
            return await SessionExecuteAction();
        }

        /// <summary>
        /// Return a UssdResponse.
        /// If <paramref name="nextRoute"/> is not provided
        /// UssdResponse.Type is set to Release.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="nextRoute"></param>
        /// <returns></returns>
        public UssdResponse Response(string message, string nextRoute = null)
        {
            return UssdResponse.New(message, nextRoute);
        }
        #endregion

        #region Session Management
        /// <summary>
        /// Set the next action to route to.
        /// </summary>
        /// <param name="nextRoute">route to next action</param>
        /// <returns></returns>
        internal async Task SessionSetNextRoute(string nextRoute)
        {
            EnsureRouteExists(nextRoute);
            await Store.SetValue(NextRouteKey, nextRoute);
        }

        /// <summary>
        /// Close session.
        /// </summary>
        /// <returns></returns>
        internal async Task SessionClose()
        {
            await Store.DeleteValue(NextRouteKey);
            await Store.DeleteHash(DataBagKey);
        }

        /// <summary>
        /// Verify session exists.
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> SessionExists()
        {
            return await Store.ValueExists(NextRouteKey);
        }

        /// <summary>
        /// Set contexts's current action.
        /// </summary>
        /// <returns></returns>
        private async Task SessionSetAction()
        {
            var route = await Store.GetValue(NextRouteKey);
            EnsureRouteExists(route);
            this.Action = Routes[route];
        }

        /// <summary>
        /// Execute the current action.
        /// </summary>
        /// <returns></returns>
        internal async Task<UssdResponse> SessionExecuteAction()
        {
            await this.SessionSetAction();
            var response = await this.Action(this);
            if (!response.Release)
            {
                await this.SessionSetNextRoute(response.NextRoute);
            }
            return response;
        } 
        #endregion

        /// <summary>
        /// Throw error if specified <paramref name="route"/>
        /// does not exist.
        /// </summary>
        /// <param name="route"></param>
        public void EnsureRouteExists(string route)
        {
            var exists = Routes.ContainsKey(route);
            if (exists) return;
            throw new Exception(string.Format("Route {0} " +
                                              "does not exist.",
                route));
        }
    }
}
