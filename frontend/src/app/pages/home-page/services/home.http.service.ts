import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { environment } from 'src/environments/environment';
import { InquiryDTO } from '../models/inquiry-dto';
import { UserCountDTO } from '../models/user-count-dto';

@Injectable({
  providedIn: 'root'
})
export class HomeHttpServiceService {
  private readonly homePageWebAPIUrl: string = environment.webApiUrl;

  constructor(private httpClient: HttpClient) { }

  getInquiries(): Observable<InquiryDTO[]> {
    const httpOptions = getHttpOptionsWithAuthenticationHeader();
    return this.httpClient.get<InquiryDTO[]>(`${this.homePageWebAPIUrl}/inquiries`, httpOptions);
  }

  getUsersCount(): Observable<UserCountDTO> {
    return this.httpClient.get<UserCountDTO>(`${this.homePageWebAPIUrl}/users-count`);
  }
}
