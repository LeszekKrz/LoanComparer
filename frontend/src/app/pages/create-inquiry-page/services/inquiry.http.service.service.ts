import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { environment } from 'src/environments/environment';
import { UserInfoDTO } from '../models/user-info-dto';
import { CreateInquiryResponse } from '../models/create-inquiry-response';
import { InquiryDTO } from '../models/inquiry-dto';

@Injectable({
  providedIn: 'root'
})
export class InquiryHttpServiceService {
  private readonly inquiriesPageWebAPIUrl: string = environment.webApiUrl;

  constructor(private httpClient: HttpClient, private authenticationService: AuthenticationService) { }

  createInquiry(inquiryDTO: InquiryDTO): Observable<CreateInquiryResponse> {
    if (this.authenticationService.isUserAuthenticated()) {
      const httpOptions = getHttpOptionsWithAuthenticationHeader();
      return this.httpClient.post<CreateInquiryResponse>(`${this.inquiriesPageWebAPIUrl}/inquiries`, inquiryDTO, httpOptions);
    }
    return this.httpClient.post<CreateInquiryResponse>(`${this.inquiriesPageWebAPIUrl}/inquiries`, inquiryDTO);
  }

  getUserInfo(): Observable<UserInfoDTO> {
    const httpOptions = getHttpOptionsWithAuthenticationHeader();
    return this.httpClient.get<UserInfoDTO>(`${this.inquiriesPageWebAPIUrl}/user/info`, httpOptions);
  }
}
