import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { JobTypeDTO } from "../models/job-type-dto";

@Injectable({
  providedIn: 'root'
})
export class JobTypesHttpService {
  private readonly jobTypesUrl: string = `${environment.webApiUrl}/job-types`;

  constructor(private httpClient: HttpClient) {}

  getJobTypes(): Observable<JobTypeDTO[]> {
    return this.httpClient.get<JobTypeDTO[]>(this.jobTypesUrl);
  }
}
