import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { UserForRegistrationDTO } from "../models/user-for-registration-dto";

@Injectable({
  providedIn: 'root'
})
export class RegistrationHttpService {
  private readonly registrationPageUrl: string = `${environment.webApiUrl}/registration-page`;

  constructor(private httpClient: HttpClient) {}

  registerUser(userForRegistrationDTO: UserForRegistrationDTO): Observable<void> {
    return this.httpClient.post<void>(`${this.registrationPageUrl}/register`, userForRegistrationDTO);
  }
}
