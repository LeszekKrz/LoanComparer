import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { AuthenticationResponseDTO } from '../../authentication/models/authentication-response-dto';
import { UserForAuthenticationDTO } from '../../authentication/models/user-for-authentication-dto';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { CredentialResponse } from 'google-one-tap';
import { UserForGoogleAuthenticationDTO } from 'src/app/authentication/models/user-for-google-authentication-dto';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required)
  });
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;
  passwordInputType = 'password';
  passwordEyeIcon = 'pi-eye';
  private googleClientId = environment.googleClientId;

  constructor(private authenticationService: AuthenticationService, private messageService: MessageService, private router: Router) { }

  ngOnInit(): void {
    // @ts-ignore
    window.onGoogleLibraryLoad = () => {
      // @ts-ignore
      google.accounts.id.initialize({
        client_id: this.googleClientId,
        callback: this.handleCredentialResponse.bind(this),
        auto_select: false,
        cancel_on_tap_outside: true
      });
      // @ts-ignore
      google.accounts.id.renderButton(
      // @ts-ignore
      document.getElementById('googleButton'),
        { theme: 'outline', size: 'large', width: '100%' },
      );
      // @ts-ignore
      google.accounts.id.prompt((_: PromptMomentNotification) => {});
    };
  }

  private handleCredentialResponse(response: CredentialResponse) {
    const userForGoogleAuthentication: UserForGoogleAuthenticationDTO = {
      credential: response.credential
    };

    const login$ = this.authenticationService.loginWithGoogle(userForGoogleAuthentication).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Login successful'
        });
      }),
    );

    this.subscriptions.push(this.doWithLoading(login$).subscribe({
      next: (authenticationResponse: AuthenticationResponseDTO) => {
        localStorage.setItem('token', authenticationResponse.token);
      },
      complete: () => {
        this.authenticationService.sendAuthenticationStateChangedNotification(true);
        this.router.navigate(['home']);
      }
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

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
        });
      }),
    );
    this.subscriptions.push(this.doWithLoading(login$).subscribe({
      next: (authenticationResponse: AuthenticationResponseDTO) => {
        localStorage.setItem('token', authenticationResponse.token);
      },
      complete: () => {
        this.authenticationService.sendAuthenticationStateChangedNotification(true);
        this.router.navigate(['home']);
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
