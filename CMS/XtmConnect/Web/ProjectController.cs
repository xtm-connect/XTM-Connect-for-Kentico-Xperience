using CMS.EventLog;
using CMS.TranslationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Xtm.Connector.Models;
using XtmConnect.Config;
using XtmConnect.Factories;
using XtmConnect.Services;
using XtmConnect.XtmConnect.Models;
using XtmConnect.XtmConnect.Services.Interfaces;

namespace XtmConnect.XtmConnect.Web
{
    public class ProjectController : ApiController // Install-Package Microsoft.AspNet.WebApi.Core
    {
        private const string EVENT_SOURCE = "XTM Connect - ProjectController";
        private IXtmTranslationProviderService xtmTranslationProviderService;

        public ProjectController() : this(new ServiceFactory())
        {
        }

        internal ProjectController(IServiceFactory serviceFactory) : this(serviceFactory.CreateXtmTranslationProviderService())
        {
        }

        internal ProjectController(IXtmTranslationProviderService xtmTranslationProviderService)
        {
            this.xtmTranslationProviderService = xtmTranslationProviderService;
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult JobFinished(long xtmProjectId, long xtmJobId, long xtmCustomerId) // http://localhost/Kentico11/xtmApi/Project/JobFinished?xtmProjectId=1&xtmJobId=1&xtmCustomerId=1
        {
            try
            {
                EventLogProvider.LogInformation(EVENT_SOURCE, Constants.Module.EventCode.JobFinished, $"Callback received. ProjectId:{xtmProjectId}, JobId:{xtmJobId}, CustomerId:{xtmCustomerId}");
                CheckProjectTranslationsCompleted(xtmProjectId);
                return Ok();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(EVENT_SOURCE, Constants.Module.EventCode.JobFinishedException, ex, 0, "Xtm translation provider ProjectFinished callback exception");
                return InternalServerError();
            }
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult ProjectFinished(long xtmProjectId) // http://localhost/Kentico11/xtmApi/Project/ProjectFinished?xtmProjectId=1
        {
            try
            {
                EventLogProvider.LogInformation(EVENT_SOURCE, Constants.Module.EventCode.ProjectFinished, $"Callback received. ProjectId:{xtmProjectId}");
                CheckProjectTranslationsCompleted(xtmProjectId);
                return Ok();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(EVENT_SOURCE, Constants.Module.EventCode.ProjectFinishedException, ex, 0, "Xtm translation provider ProjectFinished callback exception");
                return InternalServerError();
            }
        }

        private void CheckProjectTranslationsCompleted(long xtmProjectId)
        {
            var serializer = new JavaScriptSerializer();
            var xtmSubmissions = GetAllXtmSubmittions()
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SubmissionTicket) && serializer.Deserialize<XtmSubmissionTicket>(x.SubmissionTicket).ProjectId == (long?)xtmProjectId)
                .ToList();

            if (xtmSubmissions.Count == 0)
            {
                Helpers.LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"CheckProjectTranslationsCompleted - xtmSubmissions.Count == 0");
            }

            var xtmTranslationService = new XtmTranslationService();
            foreach (var submission in xtmSubmissions) // there might be more than one translation submission with the same Xtm project Id (xtm project id's are not unique between XTM CLIENTS and XTM SERVERS)
            {
                xtmTranslationService.DownloadCompletedTranslation(submission);
            }

            // unspecified provider
            // Xtm Translation Service ignores siteName and checks all translation submissions
        }

        private List<TranslationSubmissionInfo> GetAllXtmSubmittions()
        {
            var kenticoTranslationProvidersIds = TranslationServiceInfoProvider
                   .GetTranslationServices()
                   .Where(x => x != null && x.TranslationServiceClassName == typeof(XtmTranslationService).FullName).Select(x => x.TranslationServiceID)
                   .ToList();

            var xtmTranslationProviders = xtmTranslationProviderService.GetAllXtmTranslationProviderFullData().ToList();

            var submissions = TranslationSubmissionInfoProvider
                .GetTranslationSubmissions()
                .Where(sub => sub != null && sub.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation && kenticoTranslationProvidersIds != null && kenticoTranslationProvidersIds.Contains(sub.SubmissionServiceID))
                .ToList();

            return submissions;
        }
    }
}