using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XtmConnect.Config
{
    internal static class Constants
    {
        internal static class Module
        {
            internal const string CodeName = "XtmConnect";
            internal const string DisplayName = "XTM Connect";
            internal const string KenticoXtmIntegrationKey = "f3f1d09ff0ee401eabc21e5eebc3f52b";

            internal static class Action
            {
                public const string Delete = "delete";
                public const string Edit = "edit";
            }

            internal static class EventCode
            {
                public const string Exception = "EXCEPTION";
                public const string JobFinished = "JobFinished";
                public const string JobFinishedException = "JobFinished - EXCEPTION";
                public const string ProjectFinished = "ProjectFinished";
                public const string ProjectFinishedException = "ProjectFinished - EXCEPTION";
            }

            internal static class Permissions
            {
                public const string Modify = "Modify";
                public const string Read = "Read";
            }

            internal static class UIElements
            {
                public const string XtmTranslationProvider_Edit = "XtmTranslationProvider_Edit";
                public const string XtmTranslationProvider_List = "XtmTranslationProvider_List";
            }

            internal static class Web
            {
                internal const string ApiRoutePrefix = "xtmApi";

                internal static class ProjectController
                {
                    internal const string Name = "Project";

                    internal static class Actions
                    {
                        public const string JobFinished = "JobFinished";
                        public const string ProjectFinished = "ProjectFinished";
                    }
                }
            }
        }
    }
}