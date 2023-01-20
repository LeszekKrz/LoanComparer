import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { InquiryDTO } from '../models/inquiry-dto';
import { ApplicationDTO } from '../models/application-dto';
import { ReviewApplicationDTO } from '../models/review-application-dto';
import { ReviewApplicationResponse } from '../models/review-application-response.dto';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';

@Injectable({
  providedIn: 'root'
})
export class AdminPanelHttpService {
  private readonly adminPanelPageWebAPIUrl: string = `${environment.webApiUrl}/admin`;

  constructor(private httpClient: HttpClient) { }

  getInquiries(): Observable<InquiryDTO[]> {
    const httpOptions = getHttpOptionsWithAuthenticationHeader();
    return this.httpClient.get<InquiryDTO[]>(`${this.adminPanelPageWebAPIUrl}/inquiries`, httpOptions);
  }

  getOfferRequests(): Observable<ApplicationDTO[]> {
    const httpOptions = getHttpOptionsWithAuthenticationHeader();
    return this.httpClient.get<ApplicationDTO[]>(`${this.adminPanelPageWebAPIUrl}/applications`, httpOptions);
  }

  getContract(offerId: string): Observable<HttpResponse<Blob>> {
    return this.httpClient.get<Blob>(
      `${this.adminPanelPageWebAPIUrl}/applications/${offerId}/document`,
      {
        observe: 'response',
        responseType: 'blob' as 'json',
        headers: new HttpHeaders({
          Authorization: localStorage.getItem('token')!
        }),
      });
  }

  reviewApplication(offerId: string, reviewApplicationDTO: ReviewApplicationDTO): Observable<ReviewApplicationResponse> {
    const httpOptions = getHttpOptionsWithAuthenticationHeader();
    return this.httpClient.put<ReviewApplicationResponse>(
      `${this.adminPanelPageWebAPIUrl}/applications/${offerId}/review`,
      reviewApplicationDTO,
      httpOptions
    );
  }
}
