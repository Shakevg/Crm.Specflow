﻿using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using Vermaat.Crm.Specflow.Entities;

namespace Vermaat.Crm.Specflow.EasyRepro
{
    internal class FormCellParsingContext
    {
        public Dictionary<string, AttributeMetadata> MetadataDic { get; set; }
        public UCIApp App { get; set; }
        public FormCell Cell { get; set; }
        public bool IsHeader { get; set; }
        public SystemFormType FormType { get; set; }

        public FormCellTabAndSectionContext FormCellTabAndSectionContext { get; set; } = new FormCellTabAndSectionContext();
    }
}
