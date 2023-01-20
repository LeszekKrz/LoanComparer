import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { getHttpOptionsWithAuthenticationHeader } from 'src/app/core/functions/get-http-options-with-authorization-header';
import { environment } from 'src/environments/environment';
import { BankOfferDTO } from '../models/bank-offer-dto';
import { RequestOfferResponse } from '../models/request-offer-response';

@Injectable({
  providedIn: 'root'
})
export class OfferHttpService {
  private readonly chooseOfferPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient, private authenticationService: AuthenticationService) { }

  getOffers(inquiryId: string): Observable<BankOfferDTO[]> {
    const getOffersWebAPIUrl: string = `${this.chooseOfferPageWebAPIUrl}/inquiries/${inquiryId}/offers`;
    if (this.authenticationService.isUserAuthenticated()) {
      const httpOptions = getHttpOptionsWithAuthenticationHeader();
      return this.httpClient.get<BankOfferDTO[]>(getOffersWebAPIUrl, httpOptions);
    }
    return this.httpClient.get<BankOfferDTO[]>(getOffersWebAPIUrl);
  }

  getContract(offerId: string): Observable<HttpResponse<Blob>> {
    return this.httpClient.get<Blob>(
      `${this.chooseOfferPageWebAPIUrl}/offers/${offerId}/document`,
      this.authenticationService.isUserAuthenticated()
      ? {
        observe: 'response',
        responseType: 'blob' as 'json',
        headers: new HttpHeaders({
          Authorization: localStorage.getItem('token')!
        }),
      }
      : {
        observe: 'response',
        responseType: 'blob' as 'json',
      });
  }

  requestOffer(offerId: string, formFile: FormData): Observable<RequestOfferResponse> {
    return this.httpClient.post<RequestOfferResponse>(
      `${this.chooseOfferPageWebAPIUrl}/offers/${offerId}/apply`,
      formFile,
      this.authenticationService.isUserAuthenticated()
      ? getHttpOptionsWithAuthenticationHeader()
      : {});
  }
}
