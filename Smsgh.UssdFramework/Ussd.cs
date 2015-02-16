using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Smsgh.UssdFramework.Stores;

namespace Smsgh.UssdFramework
{
    public abstract class Ussd
    {
        internal IStore Store { get; private set; }
        internal Dictionary<string, Func<UssdContext, Task<UssdResponse>>> Routes { get;  private set; }

        /// <summary>
        /// Initalize with a database store.
        /// </summary>
        /// <param name="store">Database store instance</param>
        protected Ussd(IStore store)
        {
            Store = store;
            Routes = new Dictionary<string, Func<UssdContext, Task<UssdResponse>>>();
        }


        #region Events
        /// <summary>
        /// Can be overridden to dynamically load appropriate screen at 
        /// intitiation.
        /// By default loads "start" route.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="route"></param>
        public virtual async Task<UssdResponse> OnInitiation(UssdContext context, 
            string route)
        {
            await context.SessionClose();
            return await context.ReRoute(route);
        }

        public virtual async Task<UssdResponse> OnResponse(UssdContext context)
        {
            var exists = await context.SessionExists();
            if (!exists)
            {
                throw new Exception("Session does not exist.");
            }
            return await context.SessionExecuteAction();
        }

        public virtual async Task<UssdResponse> OnRelease(UssdContext context)
        {
            return context.Response("Session closed.");
        } 

        public virtual async Task<UssdResponse> OnTimeout(UssdContext context)
        {
            await context.SessionClose();
            return context.Response("Session timed out.");
        } 
        #endregion

        #region Routing
        /// <summary>
        /// Adds an ActionDelegate to Routes.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="action"></param>
        public void Route(string route, Func<UssdContext, Task<UssdResponse>> action)
        {
            Routes.Add(route, action);
        }

        /// <summary>
        /// A helper to wire up a <paramref name="menu"/>'s routes.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="menu"></param>
        /// <param name="routeChoices"></param>
        public void RouteMenu(string route, Func<UssdContext, Task<string>> menu, 
            Dictionary<string, string> routeChoices)
        {
            var routerRoute = route + "-router";
            Route(route, MenuDisplay(routerRoute, menu));
            Route(routerRoute, MenuRouter(route, routeChoices));
        }

        private static Func<UssdContext, Task<UssdResponse>> MenuDisplay(string routerRoute, 
            Func<UssdContext, Task<string>> menuDisplay)
        {
            return async (c) =>
            {
                var message = await menuDisplay(c);
                return c.Response(message, routerRoute);
            };
        }

        private static Func<UssdContext, Task<UssdResponse>> MenuRouter(string menuRoute,
            IReadOnlyDictionary<string, string> routeChoices)
        {
            return async (c) =>
            {
                var choice = c.Request.SanitizedMessage;
                if (!routeChoices.ContainsKey(choice))
                {
                    throw new Exception(string.Format("No menu item exists " +
                                                      "for selection {0}.", choice));
                }
                var nextRoute = routeChoices[choice];
                return await c.ReRoute(nextRoute);
            };
        }
        #endregion

        /// <summary>
        /// Process <paramref name="request"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startRoute"></param>
        /// <returns></returns>
        public async Task<UssdResponse> Process(UssdRequest request, 
            string startRoute = "start")
        {
            var context = new UssdContext(this, request);
            try
            {
                switch (request.RequestType)
                {
                    case UssdRequestTypes.Initiation:
                        return await this.OnInitiation(context, startRoute);
                    case UssdRequestTypes.Timeout:
                        return await this.OnTimeout(context);
                    case UssdRequestTypes.Release:
                        return await this.OnRelease(context);
                    default:
                        return await this.OnResponse(context);

                }
            }
            catch (Exception e)
            {
                return UssdResponse.New(e.Message);
            }
        }
    }
}
