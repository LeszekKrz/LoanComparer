import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { environment } from "src/environments/environment";
import { AuthenticationResponseDTO } from "../models/authentication-response-dto";
import { UserForAuthenticationDTO } from "../models/user-for-authentication-dto";
import { UserForRegistrationDTO } from "../models/user-for-registration-dto";

@Injectable({
  providedIn: 'root'
})
export class AuthenticationHttpService {
  private authenticationStateChangeSubject = new Subject<boolean>();
  private authenticationStateChanged = this.authenticationStateChangeSubject.asObservable();
  private readonly registrationPageUrl: string = `${environment.webApiUrl}/registration-page`;
  private readonly loginPageUrl: string = `${environment.webApiUrl}/login-page`;

  sendAuthenticationStateChangedNotification = (isAuthenticated: boolean): void => {
    this.authenticationStateChangeSubject.next(isAuthenticated);
  }

  constructor(private httpClient: HttpClient) {}

  registerUser(userForRegistration: UserForRegistrationDTO): Observable<void> {
    return this.httpClient.post<void>(`${this.registrationPageUrl}/register`, userForRegistration);
  }

  loginUser(userForAuthentication: UserForAuthenticationDTO): Observable<AuthenticationResponseDTO> {
    return this.httpClient.post<AuthenticationResponseDTO>(`${this.loginPageUrl}/login`, userForAuthentication);
  }

}
