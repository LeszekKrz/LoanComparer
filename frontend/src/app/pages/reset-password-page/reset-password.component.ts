import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { ResetPasswordDTO } from 'src/app/authentication/models/reset-password-dto';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
  isProgressSpinnerVisible = false;
  resetPasswordForm!: FormGroup;
  containsLowerCaseLetterRegex = /[a-z]/;
  constainsUpperCaseLetterRegex = /[A-Z]/;
  containsNumberRegex: RegExp = /\d/;
  constainsSpecialCharacterRegex = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
  passwordMinLength = 8;
  newPasswordInputType = "password";
  newPasswordEyeIcon = "pi-eye";
  confirmPasswordInputType = "password";
  confirmPasswordEyeIcon = "pi-eye";
  subscriptions: Subscription[] = [];

  constructor(
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.resetPasswordForm = this.formBuilder.group({
      newPassword: new FormControl('', [
        Validators.required,
          Validators.minLength(this.passwordMinLength),
          Validators.pattern(this.containsLowerCaseLetterRegex),
          Validators.pattern(this.constainsUpperCaseLetterRegex),
          Validators.pattern(this.containsNumberRegex),
          Validators.pattern(this.constainsSpecialCharacterRegex)
      ]),
      confirmPassword: new FormControl('', Validators.required)
    }, {
      validators: this.passwordsNotMatching
    })
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  private passwordsNotMatching: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    return group.get('newPassword')!.value === group.get('confirmPassword')!.value
      ? null
      : {passwordsNotMatching: true};
  }

  onSubmit(): void {
    if (this.resetPasswordForm.invalid) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    const resetPassword: ResetPasswordDTO = {
      password: this.resetPasswordForm.get('newPassword')!.value,
      confirmPassword: this.resetPasswordForm.get('confirmPassword')!.value,
      email: this.route.snapshot.queryParams['email'],
      token: this.route.snapshot.queryParams['token']
    }

    const resetPassword$ = this.authenticationService.resetPassword(resetPassword).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Password has been reset'
        });
      })
    );

    this.subscriptions.push(this.doWithLoading(resetPassword$).subscribe({
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

  hideShowNewPassword(): void {
    if (this.newPasswordInputType == "password") {
      this.newPasswordInputType = "text";
      this.newPasswordEyeIcon = "pi-eye-slash";
    }
    else {
      this.newPasswordInputType = "password";
      this.newPasswordEyeIcon = "pi-eye";
    }
  }

  hideShowConfirmPassword(): void {
    if (this.confirmPasswordInputType == "password") {
      this.confirmPasswordInputType = "text";
      this.confirmPasswordEyeIcon = "pi-eye-slash";
    }
    else {
      this.confirmPasswordInputType = "password";
      this.confirmPasswordEyeIcon = "pi-eye";
    }
  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.resetPasswordForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.resetPasswordForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isNewPasswordInputTouchedOrDirtyAndTooShort(): boolean {
    const control = this.resetPasswordForm.get('newPassword')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('minlength');
  }

  isNewPasswordInputTouchedOrDirtyAndDoesntContainLowerCaseLetter(): boolean {
    const control = this.resetPasswordForm.get('newPassword')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsLowerCaseLetterRegex.toString();
  }

  isNewPasswordInputTouchedOrDirtyAndDoesntContainUpperCaseLetter(): boolean {
    const control = this.resetPasswordForm.get('newPassword')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsUpperCaseLetterRegex.toString();
  }

  isNewPasswordInputTouchedOrDirtyAndDoesntContainNumber(): boolean {
    const control = this.resetPasswordForm.get('newPassword')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsNumberRegex.toString();
  }

  isNewPasswordInputTouchedOrDirtyAndDoesntContainSpecialCharacter(): boolean {
    const control = this.resetPasswordForm.get('newPassword')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsSpecialCharacterRegex.toString();
  }

  isConfirmPasswordInputInvalidAndTouchedOrDirty(): boolean {
    const control = this.resetPasswordForm.get('confirmPassword')!;
    return this.isInputTouchedOrDirty(control) && (control.invalid || this.resetPasswordForm.hasError('passwordsNotMatching'));
  }

  arePasswordAndConfirmPasswordTouchedOrDirtyAndNotMatching(): boolean {
    const passwordControl = this.resetPasswordForm.get('newPassword')!;
    const confirmPasswordControl = this.resetPasswordForm.get('confirmPassword')!;

    return this.isInputTouchedOrDirty(passwordControl)
      && this.isInputTouchedOrDirty(confirmPasswordControl)
      && this.resetPasswordForm.hasError('passwordsNotMatching');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }
}
