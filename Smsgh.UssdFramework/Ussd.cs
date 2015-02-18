using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Smsgh.UssdFramework.Stores;

namespace Smsgh.UssdFramework
{
    public class Ussd
    {
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
            await context.SessionSetNextRoute(route);
            return await this.OnResponse(context);
        }

        public virtual async Task<UssdResponse> OnResponse(UssdContext context)
        {
            while (true)
            {
                var exists = await context.SessionExists();
                if (!exists)
                {
                    throw new Exception("Session does not exist.");
                }
                var response = await context.SessionExecuteAction();
                if (!response.IsRelease)
                {
                    await context.SessionSetNextRoute(response.NextRoute);
                }
                if (response.IsRedirect)
                {
                    continue;
                }
                return response;
            }
        }
        #endregion

        /// <summary>
        /// Process <paramref name="request"/>
        /// </summary>
        /// <param name="store"></param>
        /// <param name="request"></param>
        /// <param name="intiationController">Initiation controller</param>
        /// <param name="intiationAction">Initiation action</param>
        /// <returns></returns>
        public async Task<UssdResponse> Process(IStore store, UssdRequest request,
            string intiationController, string intiationAction)
        {
            var context = new UssdContext(store, request);
            try
            {
                switch (request.RequestType)
                {
                    case UssdRequestTypes.Initiation:
                        var route = string.Format("{0}Controller.{1}", 
                            intiationController, intiationAction);
                        return await this.OnInitiation(context, route);
                    default:
                        return await this.OnResponse(context);
                }
            }
            catch (Exception e)
            {
                return UssdResponse.Render(e.Message);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}
