using System;
using System.Collections.Generic;

namespace ExcelComparer1
{
    public class GetXLObjClass
    {
        
    
        public string SourceFile { get; set; }
        public string DestFile { get; set; }
        public string SourceSheetName { get; set; }
        public string DestSheetName { get; set; }
        public List<string> SourceCol { get; set; }
        public List<string> DestCol { get; set; }
        public List<string> UniqueKeys { get; set; }
        public List<object> SelectedRules { get; set; }
        public List<string> FlagVariable { get; set; }
    }
    }

