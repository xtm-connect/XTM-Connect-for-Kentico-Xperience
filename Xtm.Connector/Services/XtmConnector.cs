using CMS.EventLog;
using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.ServiceModel;
using Xtm.Connector.Config.Interfaces;
using Xtm.Connector.Exceptions;
using Xtm.Connector.Factories;
using Xtm.Connector.Factories.Interfaces;
using Xtm.Connector.Logic.Services.Interface;
using Xtm.Connector.Models;
using Xtm.Connector.XtmWebService;

namespace Xtm.Connector.Logic.Services
{
    public class XtmConnector : IXtmConnector
    {
        private IXtmConnectorConfiguration config;
        private loginAPI loginApi;
        private ProjectManagerMTOMWebServiceClient webService;

        public XtmConnector(IXtmConnectorConfiguration config) : this(new SoapClientFactory(config), config)
        {
        }

        internal XtmConnector(ISoapClientFactory soapClientFactory, IXtmConnectorConfiguration config) : this(soapClientFactory.CreateXTMWebService(), config)
        {
        }

        internal XtmConnector(ProjectManagerMTOMWebServiceClient webService, IXtmConnectorConfiguration config)
        {
            this.webService = webService;
            this.config = config;
            loginApi = new loginAPI()
            {
                client = config.Client,
                username = config.UserName,
                password = config.Password,
                integrationKey = config.IntegrationKey,
            };
        }

        public void ArchiveProject(Project project)
        {
            try
            {
                var projectDescriptor = new xtmProjectDescriptorAPI { id = project.Id.GetValueOrDefault(), idSpecified = project.Id.HasValue, integrationId = project.IntegrationId };
                var activity = new xtmUpdateProjectActivityOptionsAPI() { activitySpecified = true, activity = xtmPROJECTACTIVITY.ARCHIVE };
                webService.updateProjectActivity(loginApi, new xtmProjectDescriptorAPI[] { projectDescriptor }, activity);
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public void CancelProject(Project project)
        {
            try
            {
                var projectDescriptor = new xtmProjectDescriptorAPI { id = project.Id.GetValueOrDefault(), idSpecified = project.Id.HasValue, integrationId = project.IntegrationId };
                var activity = new xtmUpdateProjectActivityOptionsAPI() { activitySpecified = true, activity = xtmPROJECTACTIVITY.CANCEL };
                webService.updateProjectActivity(loginApi, new xtmProjectDescriptorAPI[] { projectDescriptor }, activity);
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        /// <summary>
        /// Use autopopulate to let XTM create projectNames if not specified or add IDs increments if names are duplicated (useful with prefix project names only).
        /// </summary>
        public Project CreateProject(languageCODE sourceLanguage, List<languageCODE> targetLanguages, List<FileToTranslate> files, Customer customer, Template template, DateTime? deadline, string integrationId, string projectName, bool autoPopulateName, CallbackCollection callbacks)
        {
            try
            {
                var createProjectApi = new xtmProjectMTOMAPI()
                {
                    name = projectName,
                    dueDate = deadline.GetValueOrDefault(),
                    dueDateSpecified = deadline.HasValue,
                    externalDescriptor = new xtmExternalProjectDescriptorAPI() { integrationId = integrationId },
                    sourceLanguage = sourceLanguage,
                    sourceLanguageSpecified = true,
                    targetLanguages = targetLanguages.Cast<languageCODE?>().ToArray(),
                    template = template != null ? new xtmTemplateDescriptorAPI() { id = template.Id, idSpecified = true } : null,
                    customer = new xtmCustomerDescriptorAPI() { id = customer.Id, idSpecified = true },
                    translationFiles = files.Select(file => new xtmFileMTOMAPI()
                    {
                        fileMTOM = file.Data,
                        fileName = file.Name,
                        targetLanguages = file.TargetLanguages.Select(lang => (languageCODE?)lang).ToArray(),
                        externalDescriptors = file.FileExternalIdMaps.Select(map =>
                            new xtmFileAPIEntry
                            {
                                key = map.TargetLanguage,
                                keySpecified = true,
                                value = new xtmExternalJobDescriptorAPI { integrationId = map.IntegrationId, }
                            }).ToArray()
                    }).ToArray(),
                    projectCallback = new xtmProjectCallbackAPI { projectFinishedCallback = callbacks.ProjectFinishedCallback, jobFinishedCallback = callbacks.JobFinishedCallback }
                };

                var createProjectResult = webService.createProjectMTOM(loginApi, createProjectApi, new xtmCreateProjectMTOMOptionsAPI() { autopopulateSpecified = autoPopulateName, autopopulate = autoPopulateName });

                var resultProject = new Project()
                {
                    Id = createProjectResult.project.projectDescriptor.idSpecified == true ? (long?)createProjectResult.project.projectDescriptor.id : null,
                    IntegrationId = createProjectResult.project.projectDescriptor.integrationId,
                    Name = createProjectResult.project.name,
                    Jobs = createProjectResult.project.jobs.Select(j => new Job
                    {
                        IntegrationId = j.jobDescriptor.integrationId,
                        Id = j.jobDescriptor.idSpecified == true ? (long?)j.jobDescriptor.id : null
                    }).ToList()
                };

                return resultProject;
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public void DeleteProject(Project project)
        {
            EventLogProvider.LogInformation("XtmConnector", "Create Submission", $"DeleteProject is started for project= integrational:{project.IntegrationId}, id:{project.Id}, name:{project.Name}");
            try
            {
                var projectDescriptor = new xtmProjectDescriptorAPI { id = project.Id.GetValueOrDefault(), idSpecified = project.Id.HasValue, integrationId = project.IntegrationId };
                var activity = new xtmUpdateProjectActivityOptionsAPI() { activitySpecified = true, activity = xtmPROJECTACTIVITY.DELETE_LEAVING_TM };
                webService.updateProjectActivity(loginApi, new xtmProjectDescriptorAPI[] { projectDescriptor }, activity);
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public List<TranslatedFile> DownloadAllProjectTargetFilesAsZIPs(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException($"XtmConnector DownloadAllProjectTargetFilesAsZIPs {nameof(project)} is null.");
            }
            try
            {
                var projectDescriptor = new xtmProjectDescriptorAPI { id = project.Id.GetValueOrDefault(), idSpecified = project.Id.HasValue, integrationId = project.IntegrationId };
                var downloadProjectResult = webService.downloadProjectMTOM(loginApi, projectDescriptor, new xtmDownloadProjectMTOMOptionsAPI());
                var files = new List<TranslatedFile>();
                if (downloadProjectResult != null && downloadProjectResult.project != null && downloadProjectResult.project.jobs != null)
                {
                    files = downloadProjectResult.project.jobs.Select(jobFile => new TranslatedFile
                    {
                        Data = jobFile.fileMTOM, // zip file
                        Name = jobFile.fileName,
                        Job = new Job { Id = jobFile.jobDescriptor.idSpecified ? (long?)jobFile.jobDescriptor.id : null, IntegrationId = jobFile.jobDescriptor.integrationId }
                    }).ToList();
                }
                else
                {
                    throw new NullReferenceException($"XtmConnector DownloadAllProjectTargetFilesAsZIPs downloadProjectResult or downloadProjectResult.project or downloadProjectResult.project.jobs is null. Project id: {project.Id}. Project name: {project.Name}");
                }
                return files;
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException($"In method DownloadAllProjectTargetFilesAsZIPs. Project id: " + project.Id + ". Project name: " + project.Name + ". " + ex.Message);
            }
        }

        public List<TranslatedFile> DownloadJobsTargetFilesAsZIPs(List<Job> jobsToDownload)
        {
            try
            {
                var jobsDescriptors = jobsToDownload
                .Select(job => new xtmJobDescriptorAPI { id = job.Id.GetValueOrDefault(), idSpecified = job.Id.HasValue, integrationId = job.IntegrationId })
                .ToArray();
                var downloadJobsResult = webService.downloadJobMTOM(loginApi, jobsDescriptors, new xtmDownloadJobMTOMOptionsAPI());
                var files = downloadJobsResult.Select(jobFile => new TranslatedFile
                {
                    Data = jobFile.fileMTOM, // zip file
                    Name = jobFile.fileName,
                    Job = new Job { Id = jobFile.jobDescriptor.idSpecified ? (long?)jobFile.jobDescriptor.id : null, IntegrationId = jobFile.jobDescriptor.integrationId }
                }).ToList();
                return files;
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public List<Customer> GetCustomers()
        {
            config.Validate();

            try
            {
                var result = webService
                    .findCustomer(loginApi, new xtmFindCustomerAPI(), new xtmFindCustomerOptionsAPI())
                    .Select(x => new Customer { Id = x.customer.id, Name = x.customer.name })
                    .ToList();

                return result;
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public xtmProjectStatusEnum GetProjectStatus(Project project)
        {
            try
            {
                var projectDescriptor = new xtmProjectDescriptorAPI { id = project.Id.GetValueOrDefault(), idSpecified = project.Id.HasValue, integrationId = project.IntegrationId };
                var findProjectResponse = webService.findProject(loginApi, new xtmFilterProjectAPI { projects = new xtmProjectDescriptorAPI[] { projectDescriptor } }, null);
                var firstProject = findProjectResponse.projects.OrderByDescending(x => x.createDate).First();
                return firstProject.status;
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public List<Template> GetTemplates(long customerId)
        {
            config.Validate();

            try
            {
                var customerToFilter = new xtmCustomerDescriptorAPI[] { new xtmCustomerDescriptorAPI() { id = customerId, idSpecified = true } };

                var result = webService
                    .findTemplate(loginApi, new xtmFindTemplateAPI() { customers = customerToFilter }, new xtmFindTemplateOptionsAPI())
                    .Select(x => new Template { Id = x.template.id, Name = x.template.name })
                    .ToList();

                return result;
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public List<Job> GetTranslatedJobsInfo(Project project)
        {
            try
            {
                var projectDescriptor = new xtmProjectDescriptorAPI { id = project.Id.GetValueOrDefault(), idSpecified = project.Id.HasValue, integrationId = project.IntegrationId };
                var checkProjectCompletionResult = webService.checkProjectCompletion(loginApi, projectDescriptor, new xtmCheckProjectCompletionOptionsAPI());
                var jobs = checkProjectCompletionResult
                    .project
                    .jobs.Where(x => x.status == xtmJOBCOMPLETIONSTATUS.FINISHED)
                    .Select(x => new Job { Id = x.jobDescriptor.idSpecified ? (long?)x.jobDescriptor.id : null, IntegrationId = x.jobDescriptor.integrationId })
                    .ToList();

                return jobs;
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
        }

        public bool ProjectExists(Project project)
        {
            string EVENT_SOURCE = "XTM Connect - XtmConnector - ProjectExists";
            EventLogProvider.LogInformation(EVENT_SOURCE, "Create Submission", $"DeleteProject is started for project= integrational:{project.IntegrationId}, id:{project.Id}, name:{project.Name}");
            try
            {
                var returnValue = false;
                if (project != null)
                {
                    var projectDescriptor = new xtmProjectDescriptorAPI { id = project.Id.GetValueOrDefault(), idSpecified = project.Id.HasValue, integrationId = project.IntegrationId };
                    if (webService != null)
                    {
                        var findProjectResponse = webService.findProject(loginApi, new xtmFilterProjectAPI { projects = new xtmProjectDescriptorAPI[] { projectDescriptor } }, null);
                        if (findProjectResponse != null && findProjectResponse.projects != null)
                        {
                            returnValue = findProjectResponse.projects.Any();
                        }
                        else if (findProjectResponse != null && findProjectResponse.projects == null)
                        {
                            EventLogProvider.LogInformation(EVENT_SOURCE, "Submission deleted", "Project ID was not found in XTM, but submission was removed from Kentico");
                            return false;
                        }
                        else
                        {
                            throw new NullReferenceException(String.Format(ResHelper.GetString("findProjectResponseIsNull"), nameof(findProjectResponse), project.IntegrationId, project.Id));
                        }
                    }
                    else
                    {
                        throw new NullReferenceException(String.Format(ResHelper.GetString("ProjectExists"), nameof(webService)));
                    }
                }
                else
                {
                    throw new NullReferenceException(String.Format(ResHelper.GetString("ProjectExists"), nameof(project)));
                }
                return returnValue;
            }
            catch (FaultException ex)
            {
                if (ex.Message == "DATA_NOT_FOUND") // !!
                    return false;

                throw new XtmConnectorException(ex.Message);
            }
        }

        public void TestConnection()
        {
            config.Validate();

            try
            {
                webService.getXTMInfo(loginApi, new xtmGetXTMInfoOptionsAPI());
            }
            catch (FaultException ex)
            {
                throw new XtmConnectorException(ex.Message);
            }
            catch (EndpointNotFoundException ex)
            {
                throw new XtmConnectionException(ResHelper.GetString("EndpointCannotBeReached"), innerException: ex);
            }
        }
    }
}