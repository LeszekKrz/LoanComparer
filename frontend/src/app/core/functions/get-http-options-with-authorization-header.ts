import { HttpHeaders } from "@angular/common/http";

export function getHttpOptionsWithAuthenticationHeader(): { headers: HttpHeaders } {
  return {
    headers: new HttpHeaders({
      Authorization: localStorage.getItem('token')!
    }),
  };
}
