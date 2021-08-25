export class Home{
    colMap:boolean;
    colName: boolean;
    recordCount:boolean;
    colSeq:boolean;
    dataFormat:boolean;
    flagIndicator:boolean;
    symbol:boolean;
    dupCheck:boolean;
}

export class JsonData{
    SourceFile:string;
    DistFile:string;
    SourceSheetName:string;
    DistSheetName:String;
    SourceCol:String [];
    DistCol:string[];
    UnqineKeys:string[];
    SelectedRules:string[];
}