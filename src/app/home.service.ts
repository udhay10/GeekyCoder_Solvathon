import { Injectable,Inject } from '@angular/core';
import { Home, JsonData } from './home/home.model';
import{HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable,of} from 'rxjs';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type' : 'application/json'
  })
};
@Injectable({
  providedIn: 'root'
})
export class HomeService {
 servername: string;
  // constructor(private http: HttpClient, @Inject('Base_URL') baseurl:string) {
  //   this.servername = baseurl;
  //  }
  constructor(){}
  //  save(data: any){
  //    return this.http.post(this.servername,data);
  //  }
      ColumnNames(colNames : any){
        console.log(colNames);
        //return this.http.post(this.servername , colNames);
      }
      getRuleList():string{// Observable<any>{
       // return this.http.get<any[]>(this.url+"getproducts");
       let rule='{rule1:Column Level Mapping,rule2:Column Name Comparision}';
       return rule;
      }
      sendJasonData(data:JsonData){
        console.log(JSON.stringify(data))
      }
}
