using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtm.Connector.XtmWebService;

namespace Xtm.Connector.Models
{
    public class File
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }

    public class FileToTranslate : File
    {
        public List<languageCODE> TargetLanguages { get; set; }
        public List<FileExternalIdMap> FileExternalIdMaps { get; set; }
    }

    public class TranslatedFile : File
    {
        public Job Job { get; set; }
    }

    public class FileExternalIdMap
    {
        public string IntegrationId { get; set; }
        public languageCODE TargetLanguage { get; set; }
    }
}
