import { Injectable,Inject } from '@angular/core';
import {HttpClient, HttpHeaders,HttpResponse} from '@angular/common/http';
import {Observable,of} from 'rxjs';
import { BASE_URL } from 'app.config';
import { JsonData  } from '../Models/home.model';

const headers = new HttpHeaders().set('content-type', 'application/json');
@Injectable({
  providedIn: 'root'
})
export class HomeService {
  constructor(private http: HttpClient) {
   }

      DataOnSave(objClass1 : any){
         console.log(objClass1);
         return this.http.post(BASE_URL + 'api/dataload' , objClass1 , {headers});
        //return this.http.post(this.servername + 'api/dataload' , objClass1 , {headers ,observe: 'response',responseType: "arraybuffer"});
      }
      getRuleList() : Observable<any[]>{
        console.log("Service rule");
        return this.http.get<any[]>(BASE_URL + 'api/RuleList');
      }
      sendJasonData(){
      //  // console.log(JSON.stringify(data))
      //  this.http.get(this.servername + 'api/complete');
      }
}
