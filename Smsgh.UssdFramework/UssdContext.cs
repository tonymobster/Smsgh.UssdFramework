using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Smsgh.UssdFramework.Stores;

namespace Smsgh.UssdFramework
{
    public class UssdContext : IDisposable
    {
        private string NextRouteKey { get { return Request.Mobile + "NextRoute"; } }
        private string DataBagKey { get { return Request.Mobile + "DataBag"; } }
        private Func<Task<UssdResponse>> Action { get; set; }
        public UssdRequest Request { get; private set; }
        public IStore Store { get; set; }
        public UssdDataBag DataBag { get; private set; }

        public UssdContext(IStore store, UssdRequest request)
        {
            Store = store;
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
            return UssdResponse.Render(message, nextRoute);
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
            var routeArray = route.Split('.');
            if (routeArray.Length != 2)
            {
                throw new Exception("Invalid route format. Must be `SomeController.Action`." +
                                    "Current route is " + route);
            }
            var controllerName = routeArray[0];
            var actionName = routeArray[1];
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            UssdController controller = null;
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name == controllerName 
                        && type.IsSubclassOf(typeof (UssdController)))
                    {
                        controller = (UssdController) assembly.CreateInstance(type.FullName);
                    }
                }
            }
            if (controller == null)
            {
                throw new Exception(controllerName + " could not be found.");
            }
            controller.Request = Request;
            controller.DataBag = DataBag;
            var methodInfo = controller.GetType().GetMethod(actionName);
            if (methodInfo == null)
            {
                throw new Exception(string.Format("{0} does not have method {1}.",
                    controllerName, actionName));
            }
            this.Action = async () =>
            {
                object[] args = {};
                var response = (Task<UssdResponse>) methodInfo.Invoke(controller, args);
                return await response;
            };
        }

        /// <summary>
        /// Execute the current action.
        /// </summary>
        /// <returns></returns>
        internal async Task<UssdResponse> SessionExecuteAction()
        {
            await this.SessionSetAction();
            return await this.Action();
        } 
        #endregion

        public void Dispose()
        {
            Store.Dispose();
        }
    }
}
