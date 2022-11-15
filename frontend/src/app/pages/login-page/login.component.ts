import { Component, ViewEncapsulation } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { AuthenticationResponseDTO } from '../../authentication/models/authentication-response-dto';
import { UserForAuthenticationDTO } from '../../authentication/models/user-for-authentication-dto';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required)
  });
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;
  passwordInputType = "password";
  passwordEyeIcon = "pi-eye";

  constructor(private authenticationService: AuthenticationService, private messageService: MessageService, private router: Router) { }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const userForAuthentication: UserForAuthenticationDTO = {
      email: this.loginForm.get('email')!.value,
      password: this.loginForm.get('password')!.value
    };

    const login$ = this.authenticationService.loginUser(userForAuthentication).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Login successful'
        })
      })
    );
    this.subscriptions.push(this.doWithLoading(login$).subscribe({
      next: (authenticationResponse: AuthenticationResponseDTO) => {
        localStorage.setItem("token", authenticationResponse.token!);
      },
      complete: () => {
        this.authenticationService.sendAuthenticationStateChangedNotification(true);
        this.router.navigate(["home"]);
      }
    }));
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  hideShowPassword(): void {
    if (this.passwordInputType == "password") {
      this.passwordInputType = "text";
      this.passwordEyeIcon = "pi-eye-slash";
    }
    else {
      this.passwordInputType = "password";
      this.passwordEyeIcon = "pi-eye";
    }
  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.loginForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.loginForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isEmailInputTouchedOrDirtyAndNotValidEmail(): boolean {
    const control = this.loginForm.get('email')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('email');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }

}
