import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Observable, Subject } from "rxjs";
import { ErrorResponseDTO } from "src/app/core/models/error-response-dto";
import { environment } from "src/environments/environment";
import { AuthenticationResponseDTO } from "../models/authentication-response-dto";
import { ForgotPasswordDTO } from "../models/forgot-password-dto";
import { UserForAuthenticationDTO } from "../models/user-for-authentication-dto";
import { UserForRegistrationDTO } from "../models/user-for-registration-dto";

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private authenticationStateChangeSubject = new Subject<boolean>();
  public authenticationStateChanged = this.authenticationStateChangeSubject.asObservable();
  private readonly bankEmployeeRoleName = 'BankEmployee';
  private readonly roleUrl = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  private readonly registrationPageWebAPIUrl: string = `${environment.webApiUrl}/registration-page`;
  private readonly loginPageWebAPIUrl: string = `${environment.webApiUrl}/login-page`;
  private readonly forgotPasswordPageWebAPIUrl: string = `${environment.webApiUrl}/forgot-password-page`;

  sendAuthenticationStateChangedNotification = (isAuthenticated: boolean): void => {
    this.authenticationStateChangeSubject.next(isAuthenticated);
  }

  constructor(private httpClient: HttpClient, private jwtHelperService: JwtHelperService) {}

  registerUser(userForRegistration: UserForRegistrationDTO): Observable<ErrorResponseDTO[]> {
    return this.httpClient.post<ErrorResponseDTO[]>(`${this.registrationPageWebAPIUrl}/register`, userForRegistration);
  }

  loginUser(userForAuthentication: UserForAuthenticationDTO): Observable<AuthenticationResponseDTO> {
    return this.httpClient.post<AuthenticationResponseDTO>(`${this.loginPageWebAPIUrl}/login`, userForAuthentication);
  }

  logout(): void {
    localStorage.removeItem('token');
    this.sendAuthenticationStateChangedNotification(false);
  }

  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    return token != null && !this.jwtHelperService.isTokenExpired(token);
  }

  isUserBankEmployee(): boolean {
    const token = localStorage.getItem("token");
    if (!token) {
      return false;
    }
    const decodedToken = this.jwtHelperService.decodeToken(token);
    const role: string[] | string = decodedToken[this.roleUrl];
    if (typeof role === 'string') {
      return role === this.bankEmployeeRoleName;
    }
    return role.find(role => role === this.bankEmployeeRoleName) !== undefined;
  }

  forgotPassword(forgotPassword: ForgotPasswordDTO): Observable<ErrorResponseDTO[]> {
    return this.httpClient.post<ErrorResponseDTO[]>(`${this.forgotPasswordPageWebAPIUrl}/forgot-password`, forgotPassword);
  }
}
