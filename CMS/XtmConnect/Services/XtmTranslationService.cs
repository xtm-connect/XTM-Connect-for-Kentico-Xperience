using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.TranslationServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Xtm.Connector.Config;
using Xtm.Connector.Config.Interfaces;
using Xtm.Connector.Exceptions;
using Xtm.Connector.Logic.Services;
using Xtm.Connector.Logic.Services.Interface;
using Xtm.Connector.Models;
using Xtm.Connector.XtmWebService;
using XtmConnect.Config;
using XtmConnect.Enums;
using XtmConnect.Factories;
using XtmConnect.XtmConnect.Exceptions;
using XtmConnect.XtmConnect.Helpers;
using XtmConnect.XtmConnect.Models;
using XtmConnect.XtmConnect.Services.Interfaces;

using XTM = Xtm.Connector.Models;

namespace XtmConnect.Services
{
    /// <summary>
    /// Implementation of Kentico's abstract class code used by every "Xtm Translation Provider".
    /// </summary>
    public class XtmTranslationService : AbstractHumanTranslationService
    {
        private const string EVENT_SOURCE = "XtmTranslationService";

        // factory is needed: cannot inject XTM Connector in constructor, as it depends on configuration that is not known until the provider is resolved
        private IServiceFactory serviceFactory;

        private IXtmTranslationProviderService xtmTranslationProviderService;

        #region Constructors

        public XtmTranslationService() : this(new ServiceFactory())
        {
        }

        internal XtmTranslationService(IServiceFactory serviceFactory) : this(serviceFactory, serviceFactory.CreateXtmTranslationProviderService())
        {
        }

        internal XtmTranslationService(IServiceFactory serviceFactory, IXtmTranslationProviderService xtmTranslationProviderService)
        {
            this.serviceFactory = serviceFactory;
            this.xtmTranslationProviderService = xtmTranslationProviderService;
        }

        #endregion Constructors

        public override string CancelSubmission(TranslationSubmissionInfo submission)
        {
            try
            {
                var xtmProvider = xtmTranslationProviderService.GetXtmTranslationProviderFullDataByTranslationService(submission.SubmissionServiceID);
                var connectorConfig = CreateXtmConnectorConfiguration(xtmProvider);
                IXtmConnector xtmConnector = new XtmConnector(connectorConfig);

                var project = new Project() { IntegrationId = submission.SubmissionID.ToString() };

                if (xtmConnector.ProjectExists(project))
                    xtmConnector.ArchiveProject(project); // instead of cancel (not working yet)
                //  xtmConnector.CancelProject(project);

                return null;
            }
            catch (Exception ex)
            {
                TranslationServiceHelper.LogEvent(ex);

                if (ex is XtmModuleException || ex is XtmConnectorException) // user friendly exception message expected
                    throw new Exception(ex.Message); // 'throw' aborts the action; with user friendly message
                else
                    throw new Exception(ResHelper.GetString("ErrorCancellingTranslation")); // 'throw' aborts the action; with replaced, user friendly message
            }
        }

        public override string CreateSubmission(TranslationSubmissionInfo submission)
        {
            bool isResubmit = !string.IsNullOrEmpty(submission.SubmissionTicket);
            bool oldProjectDeletedOnResubmit = false;
            Project oldProject = new Project() { IntegrationId = "9999" };
            XtmSubmissionTicket ticket = new XtmSubmissionTicket();
            try
            {
                var xtmProvider = xtmTranslationProviderService.GetXtmTranslationProviderFullDataByTranslationService(submission.SubmissionServiceID);
                var connectorConfig = CreateXtmConnectorConfiguration(xtmProvider);
                IXtmConnector xtmConnector = new XtmConnector(connectorConfig);

                var sourceLang = submission.SubmissionSourceCulture.ToXtmCultureCode();
                var targetLangs = submission.SubmissionTargetCultures.Select(lang => lang.ToXtmCultureCode());
                var submissionItems = TranslationSubmissionItemInfoProvider
                    .GetTranslationSubmissionItems()
                    .Where(x => x.SubmissionItemSubmissionID == submission.SubmissionID)
                    .ToList();

                var files = submissionItems.Select(item => new XTM.FileToTranslate
                {
                    Name = $"{Regex.Replace(item.SubmissionItemName, @"[\\/:*?""<>|]", "")}_{submission.SubmissionSourceCulture}_{item.SubmissionItemTargetCulture}_{item.SubmissionItemID}.{item.SubmissionItemType}",
                    TargetLanguages = new List<languageCODE> { item.SubmissionItemTargetCulture.ToXtmCultureCode() },
                    Data = TranslationServiceHelper.GetTranslationsEncoding(submission.SubmissionSiteID).GetBytes(item.SubmissionItemSourceXLIFF),
                    FileExternalIdMaps = new List<XTM.FileExternalIdMap> { new XTM.FileExternalIdMap { IntegrationId = item.SubmissionItemID.ToString(), TargetLanguage = item.SubmissionItemTargetCulture.ToXtmCultureCode() } }
                }).ToList();

                var customer = new Customer() { Id = xtmProvider.XtmTranslationProvider.XTMCustomerID };
                var template = xtmProvider.XtmTranslationProvider.XTMTemplateID != 0 ? new Template() { Id = xtmProvider.XtmTranslationProvider.XTMTemplateID } : null;
                var prefix = xtmProvider.XtmTranslationProvider.XTMProjectNamePrefix;

                string projectName = null;

                if (isResubmit)
                {
                    projectName = (new JavaScriptSerializer().Deserialize<XtmSubmissionTicket>(submission.SubmissionTicket)).ProjectName;

                    oldProject = new Project() { IntegrationId = submission.SubmissionID.ToString() };
                    if (xtmConnector.ProjectExists(oldProject))
                    {
                        xtmConnector.DeleteProject(oldProject);
                        oldProjectDeletedOnResubmit = true;
                    }
                }
                else
                {
                    projectName = Regex.Replace(prefix + "_" + submission.SubmissionName, @"[\\/:*?""<>|{}]", "");
                }

                var deadline = xtmProvider.DeadlineEnabled == true ? (DateTime?)submission.SubmissionDeadline : null;

                var baseCallbackUrl = GetRootUrl() + "/" + Constants.Module.Web.ApiRoutePrefix + "/" + Constants.Module.Web.ProjectController.Name;

                //var baseCallbackUrl = URLHelper.GetFullApplicationUrl() + "/" + Constants.Module.Web.ApiRoutePrefix + "/" + Constants.Module.Web.ProjectController.Name;
                var callbackUrls = new CallbackCollection
                {
                    ProjectFinishedCallback = baseCallbackUrl + "/" + Constants.Module.Web.ProjectController.Actions.ProjectFinished,
                    JobFinishedCallback = baseCallbackUrl + "/" + Constants.Module.Web.ProjectController.Actions.JobFinished,
                };

                _ = files ?? throw new ArgumentNullException(nameof(files));
                _ = customer ?? throw new ArgumentNullException(nameof(customer));

                _ = submission ?? throw new ArgumentNullException(nameof(submission));
                _ = projectName ?? throw new ArgumentNullException(nameof(projectName));
                _ = callbackUrls ?? throw new ArgumentNullException(nameof(callbackUrls));

                var project = xtmConnector.CreateProject(sourceLang, targetLangs.ToList(), files, customer, template, deadline, submission.SubmissionID.ToString(), projectName, true, callbackUrls);

                ticket.ProjectId = project.Id;
                ticket.ProjectIntegrationId = project.IntegrationId;
                ticket.ProjectName = project.Name;

                submission.SubmissionTicket = new JavaScriptSerializer().Serialize(ticket);
                // although we use integrationId to associate Kentico submission with Xtm project,
                // project name and standard id will be kept in Ticket so that in resubmit action the project with the same name can be recreated (+ these data are needed in callbacks)

                return null;
            }
            catch (Exception ex)
            {
                string info = "";
                TranslationServiceHelper.LogEvent(ex);
                if (!isResubmit) // on first project creation
                {
                    submission.Delete();
                    info = ResHelper.GetString("ErrorSubmiting");
                }
                else  // on resubmit
                {
                    if (oldProjectDeletedOnResubmit)  // something gone wrong but old XTM project got already deleted
                    {
                        submission.SubmissionStatus = TranslationStatusEnum.SubmissionError;
                        submission.Update();
                    }
                    info = ResHelper.GetString("ErrorResubmiting");
                }

                info = info + "SubmissionID: " + submission.SubmissionID + ". Submission name: " + submission.SubmissionName + ". ";

                if (ex is XtmModuleException || ex is XtmConnectorException) // user friendly exception message expected
                    return info + ex.Message;

                EventLogProvider.LogInformation(EVENT_SOURCE, "Create Submission", $"IsResubmit={isResubmit}, oldProject={oldProject.IntegrationId}, " +
                    $"oldProjectDeleted={oldProjectDeletedOnResubmit}, ticketProjId={ticket.ProjectId}, ticketIntegrId={ticket.ProjectIntegrationId} " +
                    $"submissionStatus={submission.SubmissionStatus} ");

                return info + ResHelper.GetString("PleaseSeeExceptionLog");
            }
        }

        public override string DownloadCompletedTranslations(string siteName)
        {
            var exceptions = new List<Exception>();
            try
            {
                var kenticoTranslationProvidersIds = TranslationServiceInfoProvider
                    .GetTranslationServices()
                    .Where(x => x.TranslationServiceClassName == typeof(XtmTranslationService).FullName).Select(x => x.TranslationServiceID).ToList();

                var xtmTranslationProviders = xtmTranslationProviderService.GetAllXtmTranslationProviderFullData().ToList();

                var submissions = TranslationSubmissionInfoProvider
                    .GetTranslationSubmissions()
                    .Where(sub => sub.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation && kenticoTranslationProvidersIds.Contains(sub.SubmissionServiceID))
                    .ToList();

                foreach (var submission in submissions)
                {
                    try
                    {
                        DownloadCompletedTranslation(submission, xtmTranslationProviders);
                    }
                    catch (AggregateException ex)
                    {
                        exceptions.AddRange(ex.InnerExceptions);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            if (exceptions.Count > 1)
            {
                foreach (var exception in exceptions)
                    TranslationServiceHelper.LogEvent(exception);

                return String.Format(ResHelper.GetString("ErrorsOccured"), exceptions.Count);
            }
            else if (exceptions.Count == 1)
            {
                var singleEx = exceptions.Single();
                TranslationServiceHelper.LogEvent(singleEx);
                if (singleEx is XtmModuleException || singleEx is XtmConnectorException) // show message only if friendly
                    return singleEx.Message;

                return String.Format(ResHelper.GetString("ErrorsOccured"), exceptions.Count);
            }
            else // no exceptions occured
            {
                return null;
            }
        }

        public override bool IsAvailable()
        {
            return true;
        }

        public override bool IsSourceLanguageSupported(string langCode)
        {
            return CheckLanguageSupportInXTM(langCode);
        }

        public override bool IsTargetLanguageSupported(string langCode)
        {
            return CheckLanguageSupportInXTM(langCode);
        }

        internal void DownloadCompletedTranslation(TranslationSubmissionInfo submission)
        {
            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s)-Starting. Submission id: {submission.SubmissionID} looking for providers");
            List<XtmTranslationProviderFullData> xtmTranslationProviders = xtmTranslationProviderService.GetAllXtmTranslationProviderFullData().ToList();
            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s)-Starting. {xtmTranslationProviders.Count} providers found");
            this.DownloadCompletedTranslation(submission, xtmTranslationProviders);
        }

        /// <summary>
        /// Checks if translation is completed in XTM according to import type and downloads translated content from XTM.
        /// </summary>
        /// <param name="submission"></param>
        /// <param name="xtmTranslationProviders">This method is usually invoked in a loop, therefore providing XTM translation providers list param fastens processing time by reducing DB queries.</param>
        internal void DownloadCompletedTranslation(TranslationSubmissionInfo submission, List<XtmTranslationProviderFullData> xtmTranslationProviders)
        {
            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-Starting. Submission id: {submission.SubmissionID}");
            var xtmProvider = xtmTranslationProviders.SingleOrDefault(x => x.XtmTranslationProvider.TranslationProviderID == submission.SubmissionServiceID);
            if (xtmProvider == null)
                throw new XtmModuleException($"Xtm translation provider was deleted. Cannot process submission {submission.SubmissionID}");

            var connectorConfig = CreateXtmConnectorConfiguration(xtmProvider);
            IXtmConnector xtmConnector = new XtmConnector(connectorConfig);
            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-XtmConnector created.");

            var project = new Project() { IntegrationId = submission.SubmissionID.ToString() };
            if (!xtmConnector.ProjectExists(project))
            {
                // TranslationSubmissionInfoProvider.DeleteTranslationSubmissionInfo(submission.SubmissionID);
                // do not delete submission, as it can still be resubmitted
                throw new XtmModuleException($"XTM project (Submission ID: {submission.SubmissionID}, NAME: {submission.SubmissionTicket}) was deleted. You can resubmit, cancel or delete submission.");
            }

            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy  {xtmProvider.XtmTranslationProvider.TranslationImportType}.");
            if (xtmProvider.XtmTranslationProvider.TranslationImportType == (int)TranslationImportType.CheckProjectCompletion)
            {
                LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-ChPrCo.");
                if (xtmConnector.GetProjectStatus(project) == xtmProjectStatusEnum.FINISHED)
                {
                    LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-ChPrCo-FINISHED. Calling DownloadAllProjectTargetFilesAsZIPs.");
                    var targetFilesZIPs = xtmConnector.DownloadAllProjectTargetFilesAsZIPs(project);
                    LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-ChPrCo-FINISHED. Called DownloadAllProjectTargetFilesAsZIPs.");

                    using (CMSTransactionScope trans = new CMSTransactionScope())
                    {
                        foreach (var file in targetFilesZIPs)
                        {
                            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-ChPrCo-FINISHED.ImportXLIFFfromZIP for {file.Name}.");
                            string err = TranslationServiceHelper.ImportXLIFFfromZIP(submission, new MemoryStream(file.Data));
                            if (err != null)
                            {
                                throw new XtmModuleException($"XLIFF import exception message: {err}");
                            }
                        }

                        LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-ChPrCo-FINISHED.Creating submissionItemsAwaitingTranslation");
                        var submissionItemsAwaitingTranslation = TranslationSubmissionItemInfoProvider
                            .GetTranslationSubmissionItems()
                            .Where(x => x.SubmissionItemSubmissionID == submission.SubmissionID)
                            .ToList();

                        LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-ChPrCo-FINISHED.Setting TranslationReady");
                        submission.SubmissionStatus = TranslationStatusEnum.TranslationReady;
                        TranslationSubmissionInfoProvider.SetTranslationSubmissionInfo(submission);
                        trans.Commit();
                        LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-ChPrCo-FINISHED.trans.Commit() called");
                    }
                }
            }
            else if (xtmProvider.XtmTranslationProvider.TranslationImportType == (int)TranslationImportType.CheckJobCompletion)
            {
                LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion.");
                var importExceptions = new List<Exception>();

                var submissionItemsAwaitingTranslation = TranslationSubmissionItemInfoProvider
                    .GetTranslationSubmissionItems()
                    .Where(x => x.SubmissionItemSubmissionID == submission.SubmissionID && string.IsNullOrEmpty(x.SubmissionItemTargetXLIFF))
                    .ToList();
                LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion. submissionItemsAwaitingTranslation created.");

                List<Job> allFinishedJobs = xtmConnector.GetTranslatedJobsInfo(project);
                List<Job> jobsToDownload = allFinishedJobs
                    .Where(x => submissionItemsAwaitingTranslation.Any(y => y.SubmissionItemID == int.Parse(x.IntegrationId)))
                    .ToList(); // all jobs that are finished in XTM but were not downloaded yet (no xliff in Kentico)
                LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion. jobsToDownload created.");

                if (jobsToDownload.Any())
                {
                    LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion. jobsToDownload.Any() is true calling DownloadJobsTargetFilesAsZIPs.");
                    var targetFilesZIPs = xtmConnector.DownloadJobsTargetFilesAsZIPs(jobsToDownload);
                    LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion. DownloadJobsTargetFilesAsZIPs called.");

                    using (CMSTransactionScope trans = new CMSTransactionScope())
                    {
                        foreach (var file in targetFilesZIPs)
                        {
                            try
                            {
                                LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion. Calling ImportXLIFFfromZIP for {file.Name}.");
                                string err = TranslationServiceHelper.ImportXLIFFfromZIP(submission, new MemoryStream(file.Data));
                                if (err != null)
                                {
                                    throw new XtmModuleException($"XLIFF import exception message: {err}");
                                }
                            }
                            catch (Exception ex)
                            {
                                importExceptions.Add(ex);
                            }
                        }

                        LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion.Creating submissionItemsAwaitingTranslation");

                        submissionItemsAwaitingTranslation = TranslationSubmissionItemInfoProvider
                            .GetTranslationSubmissionItems()
                            .Where(x => x.SubmissionItemSubmissionID == submission.SubmissionID)
                            .ToList();

                        if (importExceptions.Count > 0)
                        {
                            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion. importExceptions were found throwing exception.");
                            throw new AggregateException(importExceptions);
                        }

                        if (xtmConnector.GetProjectStatus(project) == xtmProjectStatusEnum.FINISHED) // project needs to be finished
                        {
                            submission.SubmissionStatus = TranslationStatusEnum.TranslationReady;
                            TranslationSubmissionInfoProvider.SetTranslationSubmissionInfo(submission);
                            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion. SubmissionStatus set to TranslationReady.");
                        }

                        trans.Commit();
                        LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-TrImTy-CheckJobCompletion.trans.Commit() called");
                    }
                }
            }
            LogExtension.SafelyLogToKentico(EventType.INFORMATION, EVENT_SOURCE, $"DCT(s,x){submission.SubmissionID}-Ended. Submission id: {submission.SubmissionID}");
        }

        private bool CheckLanguageSupportInXTM(string langCode)
        {
            try
            {
                var xtmCultureCode = langCode.ToXtmCultureCode();
                return true;
            }
            catch (Exception ex)
            {
                TranslationServiceHelper.LogEvent(ex);

                if (ex is XtmModuleException)
                    return false;

                throw new Exception(String.Format(ResHelper.GetString("ErrorVerifying"), langCode));
            }
        }

        private IXtmConnectorConfiguration CreateXtmConnectorConfiguration(XtmTranslationProviderFullData xtmProviderFullData)
        {
            var xtmProviderDetails = xtmProviderFullData.XtmTranslationProvider;
            return new XtmConnectorConfiguration(xtmProviderDetails.XTMClient, xtmProviderDetails.XTMUsername, xtmProviderDetails.XTMPassword, xtmProviderDetails.XTMWebserviceURI, Constants.Module.KenticoXtmIntegrationKey);
        }

        private string GetRootUrl()
        {
            var requestUrl = CMS.Helpers.RequestContext.URLReferrer;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            var baseUrl = new Uri(string.Format("{0}://{1}", new Uri(requestUrl).Scheme, new Uri(requestUrl).Authority));
            var fullBaseUrl = new Uri(baseUrl, appUrl);

            return fullBaseUrl.ToString().Trim('/');
        }
    }
}