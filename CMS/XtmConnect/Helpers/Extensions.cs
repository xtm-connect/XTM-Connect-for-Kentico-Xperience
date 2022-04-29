using CMS.EventLog;
using System;
using XtmConnect.Config;
using XtmConnect.XtmConnect.Exceptions;
using XtmSystem = Xtm.Connector.XtmWebService;

namespace XtmConnect.XtmConnect.Helpers
{
    public static class LogExtension
    {
        public static void SafelyLogToKentico(string eventType, string source, string eventCode, string eventDescription = null)
        {
            if (eventCode.Length > 99)
            {
                eventDescription = $"{eventCode} {eventDescription}";
                eventCode = eventCode.Substring(0, 99);
            }
            EventLogProvider.LogEvent(eventType, source, eventCode, eventDescription);
        }
    }

    internal static class XtmExtensions
    {
        internal static string ToKenticoCultureCode(this XtmSystem.languageCODE xtmCultureCode)
        {
            return xtmCultureCode.ToString().Replace("_", "-");
        }

        internal static XtmSystem.languageCODE ToXtmCultureCode(this string kenticoCultureCode)
        {
            var xtmCultureString = kenticoCultureCode.Replace("-", "_");
            try
            {
                var xtmCulture = (XtmSystem.languageCODE)Enum.Parse(typeof(XtmSystem.languageCODE), xtmCultureString);
                return xtmCulture;
            }
            catch (ArgumentException ex)
            {
                EventLogProvider.LogException(Constants.Module.DisplayName, Constants.Module.EventCode.Exception, ex, 0, "Xtm test connection exception");
                throw new XtmModuleException($"XTM does not support {kenticoCultureCode} culture.");
            }
        }
    }
}