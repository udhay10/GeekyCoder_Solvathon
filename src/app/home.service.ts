import { Injectable,Inject } from '@angular/core';
import{HttpClient, HttpHeaders} from '@angular/common/http';

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
}
