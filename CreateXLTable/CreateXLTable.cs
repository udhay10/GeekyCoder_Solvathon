using System;
using System.Collections.Generic;

namespace CreateXLTable
{
    public class CreateXLTableClass
    {
        //public Array[] filename {get;set;}
        public  String srcFilename{get;set;}
        public  String dstnFilename{get;set;}
        public  String srcSheetname{get;set;}
        public  String dstnSheetname{get;set;}
        public List<string> srcCol{get;set;}
        public List<string> dstnCol{get;set;}
        public List<string> srcUniquekey{get;set;}
        public List<string> dstUniquekey{get;set;}
        

    }
}
