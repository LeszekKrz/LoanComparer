import { Component, OnDestroy } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { ForgotPasswordDTO } from 'src/app/authentication/models/forgot-password-dto';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnDestroy {
  email: FormControl = new FormControl('', [Validators.required, Validators.email]);
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;
  clientURI: string = environment.webUrl + '/reset-password';

  constructor(private authenticationService: AuthenticationService, private messageService: MessageService, private router: Router) { }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  onSubmit(): void {
    if (this.email.invalid) {
      this.email.markAsTouched();
      return;
    }

    const forgotPassword: ForgotPasswordDTO = {
      email: this.email.value,
      clientURI: this.clientURI
    };

    const forgotPassword$ = this.authenticationService.forgotPassword(forgotPassword).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'A password reset email has been sent to the address provided'
        })
      })
    );
    this.subscriptions.push(this.doWithLoading(forgotPassword$).subscribe({
      complete: () => {
        this.router.navigate(["login"]);
      }
    }));
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  isEmailInputInvalidAndTouchedOrDirty(): boolean {
    return this.isEmailInputTouchedOrDirty() && this.email.invalid;
  }

  isEmailInputTouchedOrDirtyAndEmpty(): boolean {
    return this.isEmailInputTouchedOrDirty() && this.email.hasError('required');
  }

  isEmailInputTouchedOrDirtyAndNotValidEmail(): boolean {
    return this.isEmailInputTouchedOrDirty() && this.email.hasError('email');
  }

  private isEmailInputTouchedOrDirty(): boolean {
    return this.email.touched || this.email.dirty;
  }
}
