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
    public static class Ussd
    {
        #region Events
        private static async Task<UssdResponse> OnInitiation(UssdContext context, 
            string route)
        {
            await context.SessionClose();
            await context.SessionSetNextRoute(route);
            return await OnResponse(context);
        }

        private static async Task<UssdResponse> OnResponse(UssdContext context)
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
        /// Process USSD
        /// </summary>
        /// <param name="store"></param>
        /// <param name="request"></param>
        /// <param name="initiationController">Initiation controller</param>
        /// <param name="initiationAction">Initiation action</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<UssdResponse> Process(IStore store, UssdRequest request,
            string initiationController, string initiationAction,
            Dictionary<string, string> data = null)
        {
            if (data == null)
            {
                data = new Dictionary<string, string>();
            }
            var context = new UssdContext(store, request, data);
            try
            {
                switch (request.RequestType)
                {
                    case UssdRequestTypes.Initiation:
                        var route = string.Format("{0}Controller.{1}", 
                            initiationController, initiationAction);
                        return await OnInitiation(context, route);
                    default:
                        return await OnResponse(context);
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
