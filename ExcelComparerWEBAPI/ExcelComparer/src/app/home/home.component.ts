import { Component, OnInit } from '@angular/core';
import * as XLSX from 'xlsx';
import {DualListComponent} from 'angular-dual-listbox';
import {CdkDragDrop,moveItemInArray} from '@angular/cdk/drag-drop';
import {JsonData} from '../../Models/home.model';
import {HomeService} from '../../Service/home.service';
import { fileURLToPath } from 'url';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  frmValid: boolean = false;
  title = 'Solvathon';
  spinnerEnabled = false;
  Sread=false;
  Dread=false;
  SourceSheetlist: string[]=[];
  DistSheetlist:string[]=[];
  isSourceExcelFile:boolean;
  isDistExcelFile:boolean;
  wbSorce: any;
  wbDist:any;
  FileName: any;
  DistCol:any;
  errorMessage:string[]=[];
  RuleList:any;
  ApiData:JsonData={
    SourceFile:undefined,
    DestFile:undefined,
    SourceSheetName:undefined,
    DestSheetName:undefined,
    SourceCol:[],
    DestCol:[],
    UniqueKeys:[],
    SelectedRules:[],
    FlagVariable:[]
  }
  constructor(private service:HomeService) {      
   }

  ngOnInit(): void {
    this.service.getRuleList().subscribe(
      (res) => {
        this.RuleList = res;
        console.log(this.RuleList);
      },
      error => {
        this.frmValid = true;
        this.errorMessage.push(error.message);
       console.error('There was an error!', error);
      });
    }

  onSourceChange(evt:any){
    const target: DataTransfer=<DataTransfer>(evt.target);
    this.isSourceExcelFile = !!target.files[0].name.match(/(.xls|.xlsx)/);
    this.ApiData.SourceFile=target.files[0].name;
    if (this.isSourceExcelFile) {
     this.spinnerEnabled = true;
     const reader: FileReader = new FileReader();
     reader.onload = (e: any) => {
       /* read workbook */
       const bstr = e.target.result;
       this.wbSorce = XLSX.read(bstr, { type: 'binary' });
       
       /* grab sheet names */
       this.SourceSheetlist = this.wbSorce.SheetNames;
       this.Sread=true;
      };
     reader.readAsBinaryString(target.files[0]);
 
     reader.onloadend = (e) => {
       this.spinnerEnabled = false;
       
     }
   }
   }
   onDistChange(evt:any){
     const target: DataTransfer=<DataTransfer>(evt.target);
    this.isDistExcelFile = !!target.files[0].name.match(/(.xls|.xlsx)/);
    this.ApiData.DestFile=target.files[0].name;
    if (this.isDistExcelFile) {
     this.spinnerEnabled = true;
     const reader: FileReader = new FileReader();
     reader.onload = (e: any) => {
       /* read workbook */
       const bstr = e.target.result;
       this.wbDist = XLSX.read(bstr, { type: 'binary' });
       
 
       /* grab sheet names */
       this.DistSheetlist = this.wbDist.SheetNames;
       this.Dread=true;
      };
 
     reader.readAsBinaryString(target.files[0]);
 
     reader.onloadend = (e) => {
       this.spinnerEnabled = false;
     }
   }
   }
   onSourceSheet(event:any){
     console.log(event.target.value);
     let sheet=event.target.value;
     let data;
     
     if (sheet != '**'){
       this.ApiData.SourceSheetName=sheet;
     const ws: XLSX.WorkSheet=this.wbSorce.Sheets[sheet];
     /*data=XLSX.utils.sheet_to_json(ws); 
     this.Sourcekeys=Object.keys(data[0]);*/
     this.ApiData.SourceCol=this.get_header_row(ws);
     console.log(this.ApiData.SourceCol);
     }
     else{
       alert('Please Select the correct Sheet');
     }
   }
   onDistSheet(event:any){
     console.log(event.target.value);
     let sheet=event.target.value;
     let data;
     
     if (sheet != '**'){
      this.ApiData.DestSheetName=sheet;
     const ws: XLSX.WorkSheet=this.wbDist.Sheets[sheet];
     data=XLSX.utils.sheet_to_json(ws); 
     this.ApiData.DestCol=this.get_header_row(ws);
     console.log(this.ApiData.DestCol);
     }
     else{
       alert('Please Select the correct Sheet');
     }
   }
   drop(event: CdkDragDrop<string[]>) {
     moveItemInArray(this.ApiData.DestCol, event.previousIndex, event.currentIndex);
     console.log(this.ApiData.DestCol);
   }
   get_header_row(sheet:any) {
     var headers = [];
     var range = XLSX.utils.decode_range(sheet['!ref']);
     var C, R = range.s.r; /* start in the first row */
     /* walk every column in the range */
     for (C = range.s.c; C <= range.e.c; ++C) {
       var cell = sheet[XLSX.utils.encode_cell({ c: C, r: R })] /* find the cell in the first row */
        //console.log("cell",cell)
       var hdr = "UNKNOWN " + C; // <-- replace with your desired default 
       if (cell && cell.t) {
         hdr = XLSX.utils.format_cell(cell);
         headers.push(hdr);
       }
     }
     return headers;
   }
   onPush(Ch:String){
     if (Ch === 'S'){
       this.ApiData.SourceCol.push('~');
     }
     else{
       this.ApiData.DestCol.push('~');
     }
   }
   onRemove(Ch:String){
     let index=-1;
      if(Ch === 'S'){
       index=this.ApiData.SourceCol.indexOf('~');
        if(index != -1){this.ApiData.SourceCol.splice(index,1);}
      }else{
        index=this.ApiData.DestCol.indexOf('~');
        if(index != -1){this.ApiData.DestCol.splice(index,1);}
      }
   }
   
  onStart() 
  {
    this.errorMessage=[];
    
     if(this.isDataValid()){
      this.spinnerEnabled = true;
      this.service.DataOnSave(JSON.stringify(this.ApiData)).subscribe(
        res => {
          this.spinnerEnabled = false;
           this.FileName = res;
           let name = "";
           let n = 1;
           for(var i=0; i< this.FileName.length; i++){
             name = name + n  + ") " + this.FileName[i] + "\n";
             n++;
           }        
           alert("Please find the excel in the project Output folder \n " + name)
          //this.ApiData.DestFile = res.toString();
        },
        error => {
               this.frmValid = true;
              this.errorMessage.push(error.message);
              console.error('There was an error!', error);
              this.spinnerEnabled = false;
        }
      );
     }
    
  }   
  onCheckboxChange(eve:any){
    if(eve.target.checked){
      this.ApiData.SelectedRules.push(eve.target.value);
    }else{
      let i:number=0;
      this.ApiData.SelectedRules.forEach((item:String)=>{
        if(item ==eve.target.value){
          this.ApiData.SelectedRules=this.ApiData.SelectedRules.filter((value: String)=>value!=item);
        }
      });
    }
  }
  reset(){
    console.log("Refreshed!");
    this.Sread = false;
    this.Dread=false;
    this.ApiData.SourceCol = null;
    this.SourceSheetlist=[];
    this.ApiData.DestCol=null;
    window.location.reload();
  }
  isDataValid():boolean{
    if(this.ApiData.SourceFile == undefined || this.ApiData.SourceFile == ''){
      this.frmValid = true;
      this.errorMessage.push( "* Please select the SourceFile");
    }
    if(this.ApiData.DestFile == undefined || this.ApiData.DestFile == ''){
      this.frmValid = true;
      this.errorMessage.push( "* Please select the DestinationFile");
    }
    if(this.ApiData.SourceSheetName == undefined || this.ApiData.SourceSheetName == ''){
      this.frmValid=true;
      this.errorMessage.push(" * Please Select the Source Sheet")
    }
    if(this.ApiData.DestSheetName == undefined || this.ApiData.DestSheetName ==''){
      this.frmValid=true;
      this.errorMessage.push(" * please Select the Destination Sheet");
    }
    if(this.ApiData.SourceCol.length != this.ApiData.DestCol.length){
      this.frmValid = true;
      this.errorMessage.push("* There is mismatch in the column count. Please push null to the missing columns");
    }
    console.log(this.ApiData.UniqueKeys.length)
    if(this.ApiData.UniqueKeys.length <1){
      this.frmValid=true;
      this.errorMessage.push(" * please Select at least one Uniquekey");
    }
    if(this.ApiData.SelectedRules.length< 1 ){
      this.frmValid=true;
      this.errorMessage.push(" * please Select at least one Rule from the list");
    }
    return !this.frmValid;
  }
  OnAddUnqine(e){
    if(this.ApiData.UniqueKeys.length == 0){
      if(this.checkMapping(e) ){
        if(e != 'Null'){this.ApiData.UniqueKeys.push(e);}else{
         alert('Please Select the Correct Unique Key');
        }
       
      }else{
         alert('This Column '+e+'has no mapped column in Distination');
      }
      
    }else{
      if(this.ApiData.UniqueKeys.indexOf(e)==-1){
       if(this.checkMapping(e) ){
         if(e != '~'){this.ApiData.UniqueKeys.push(e);}else{
          alert('Please Select the Correct Unique Key');
         }
        
       }else{
          alert('This Column '+e+'has no mapped column in Distination');
       }
      }else{
         alert('The Unique Key '+ e +' is already Present in the list');
      }
    }
   }
   OnRemoveUnqine(e){
         this.ApiData.UniqueKeys=this.ApiData.UniqueKeys.filter(value=>value!=e);
   }
   checkMapping(item):boolean{
     let index=this.ApiData.SourceCol.indexOf(item);
     if(this.ApiData.DestCol[index] === undefined || this.ApiData.DestCol[index] === '~'){
       return false;
     }else{
     return true;}
   }
   
}
