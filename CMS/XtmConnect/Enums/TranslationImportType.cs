using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XtmConnect.Enums
{
    internal enum TranslationImportType
    {
        [EnumStringRepresentation("Check project completion")]
        CheckProjectCompletion = 0,

        [EnumStringRepresentation("Check job completion")]
        CheckJobCompletion = 1
    }
}