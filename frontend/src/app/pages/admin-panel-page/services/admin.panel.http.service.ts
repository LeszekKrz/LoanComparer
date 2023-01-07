import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { InquiryDTO } from '../models/inquiry-dto';

@Injectable({
  providedIn: 'root'
})
export class AdminPanelHttpService {
  private readonly adminPanelPageWebAPIUrl: string = `${environment.webApiUrl}/admin`;

  constructor(private httpClient: HttpClient) { }

  getInquiries(): Observable<InquiryDTO[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        Authorization: localStorage.getItem('token')!
      }),
    };
    return this.httpClient.get<InquiryDTO[]>(`${this.adminPanelPageWebAPIUrl}/inquiries`, httpOptions);
  }
}
