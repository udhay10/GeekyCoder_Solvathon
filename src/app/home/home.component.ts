import { Component, OnInit } from '@angular/core';
import * as XLSX from 'xlsx'
import {DualListComponent} from 'angular-dual-listbox';
import {CdkDragDrop, moveItemInArray} from '@angular/cdk/drag-drop';
import { Home } from './home.model';
import { HomeService } from '../home.service';
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
  Sourcekeys: string[];
  DistKeys:any;
  SourceSheetlist: string[];
  DistSheetlist:string[];
  target:string;
  isSourceExcelFile:boolean;
  isDistExcelFile:boolean;
  SourceSheet:string;
  DistSheet:string;
  wbSorce: XLSX.WorkBook;
  wbDist:XLSX.WorkBook;
  DistCol:any;
  errorMessage: any;

   data : Home = {
    colMap:false,
    colName: false,
    recordCount:false,
    colSeq:false,
    dataFormat:false,
    flagIndicator:false,
    symbol:false,
    dupCheck:false
  }
  constructor(private service: HomeService) { }

  ngOnInit(): void {
  }

  onSourceChange(evt){
    const target: DataTransfer=<DataTransfer>(evt.target);
    this.isSourceExcelFile = !!target.files[0].name.match(/(.xls|.xlsx)/);
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
   onDistChange(evt){
     const target: DataTransfer=<DataTransfer>(evt.target);
    this.isDistExcelFile = !!target.files[0].name.match(/(.xls|.xlsx)/);
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
   onSourceSheet(event){
     console.log(event.target.value);
     let sheet=event.target.value;
     let data;
     
     if (sheet != '**'){
       
     const ws: XLSX.WorkSheet=this.wbSorce.Sheets[sheet];
     /*data=XLSX.utils.sheet_to_json(ws); 
     this.Sourcekeys=Object.keys(data[0]);*/
     this.Sourcekeys=this.get_header_row(ws);
     console.log(this.Sourcekeys);
     }
     else{
       alert('Please Select the correct Sheet');
     }
   }
   onDistSheet(event){
     console.log(event.target.value);
     let sheet=event.target.value;
     let data;
     
     if (sheet != '**'){
       
     const ws: XLSX.WorkSheet=this.wbDist.Sheets[sheet];
     data=XLSX.utils.sheet_to_json(ws); 
     this.DistKeys=this.get_header_row(ws);
     console.log(this.DistKeys);
     }
     else{
       alert('Please Select the correct Sheet');
     }
   }
   drop(event: CdkDragDrop<string[]>) {
     moveItemInArray(this.DistKeys, event.previousIndex, event.currentIndex);
     console.log(this.DistKeys);
     //this.service.ColumnNames(JSON.stringify(this.DistKeys));
    //  this.service.ColumnNames(JSON.stringify(this.DistKeys)).subscribe(
    //    data => {
    //      this.DistKeys = data;
    //    },
    //    error => {
    //      this.frmValid = true;
    //     this.errorMessage = error.message;
    //     console.error('There was an error!', error);
    // }
    //  )
   }
   get_header_row(sheet) {
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
   
  onStart()
  {

  }   
}
