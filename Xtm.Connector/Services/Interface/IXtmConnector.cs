using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtm.Connector.Models;
using Xtm.Connector.XtmWebService;

namespace Xtm.Connector.Logic.Services.Interface
{
    public interface IXtmConnector
    {
        void TestConnection();
        List<Customer> GetCustomers();
        List<Template> GetTemplates(long customerId);
        Project CreateProject(languageCODE sourceLanguage, List<languageCODE> targetLanguages, List<FileToTranslate> files, Customer customer, Template template, DateTime? deadline, string integrationId, string projectName, bool autoPopulateName, CallbackCollection callbacks);
        void DeleteProject(Project project);
        bool ProjectExists(Project project);
        void CancelProject(Project project);
        void ArchiveProject(Project project);
        xtmProjectStatusEnum GetProjectStatus(Project project);
        List<TranslatedFile> DownloadAllProjectTargetFilesAsZIPs(Project project);
        List<Job> GetTranslatedJobsInfo(Project project);
        List<TranslatedFile> DownloadJobsTargetFilesAsZIPs(List<Job> jobsToDownload);
    }
}
