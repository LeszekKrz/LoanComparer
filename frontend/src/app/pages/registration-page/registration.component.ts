import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { JobTypeDTO } from 'src/app/core/models/job-type-dto';
import { JobTypesHttpService } from 'src/app/core/services/job.type.http.service';
import { GovernmentIdDTO } from '../../authentication/models/government-id-dto';
import { UserForRegistrationDTO } from '../../authentication/models/user-for-registration-dto';
import { AuthenticationHttpService } from 'src/app/authentication/services/authentication.http.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit, OnDestroy {
  registerForm!: FormGroup;
  isPasswordVisible = false;
  isConfirmPasswordVisible = false;
  jobTypes: JobTypeDTO[] = [];
  isProgressSpinnerVisible = false;
  subscriptions: Subscription[] = [];
  governmentIdTypes = ['PESEL', 'ID Number', 'Passport Number'];
  containsLowerCaseLetterRegex = /[a-z]/;
  constainsUpperCaseLetterRegex = /[A-Z]/;
  containsNumberRegex: RegExp = /\d/;
  constainsSpecialCharacterRegex = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
  passwordMinLength = 8;
  passwordInputType = "password";
  passwordEyeIcon = "pi-eye";
  confirmPasswordInputType = "password";
  confirmPasswordEyeIcon = "pi-eye";

  constructor(private formBuilder: FormBuilder,
    private jobTypesHttpService: JobTypesHttpService,
    private authenticationHttpService: AuthenticationHttpService,
    private messageService: MessageService,
    private router: Router) { }

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      jobType: new FormControl('', Validators.required),
      incomeLevel: new FormControl(null, [Validators.required, Validators.min(1)]),
      governmentIdType: new FormControl(null, Validators.required),
      governmentIdValue: new FormControl('', Validators.required),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(this.passwordMinLength),
        Validators.pattern(this.containsLowerCaseLetterRegex),
        Validators.pattern(this.constainsUpperCaseLetterRegex),
        Validators.pattern(this.containsNumberRegex),
        Validators.pattern(this.constainsSpecialCharacterRegex)
      ]),
      confirmPassword: new FormControl('', [Validators.required])
    }, {
      validators: [
        this.passwordsNotMatching,
        this.governmentIdValueIsValid
      ]
    });

    const getJobTypes$ = this.jobTypesHttpService.getJobTypes().pipe(
      tap((jobTypes: JobTypeDTO[]) => {
        this.jobTypes = jobTypes.map(jobType => ({name: jobType.name}));
      })
    );
    this.subscriptions.push(this.doWithLoading(getJobTypes$).subscribe());
  }

  private passwordsNotMatching: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    return group.get('password')!.value === group.get('confirmPassword')!.value
      ? null
      : {passwordsNotMatching: true};
  }

  private governmentIdValueIsValid: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    var regex: RegExp;
    switch(group.get('governmentIdType')!.value) {
      case 'PESEL':
        regex = new RegExp(/^\d{11}$/);
        break;
      case 'ID Number':
        regex = new RegExp(/^[a-zA-Z]{3}\d{6}$/);
        break;
      case 'Passport Number':
        regex = new RegExp(/^[a-zA-Z]{2}\d{7}$/);
        break;
      default:
        return null;
    }
    return regex.test(group.get('governmentIdValue')!.value)
      ? null
      : {invalidGovernmentIdValue: true};
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const jobType: JobTypeDTO = {
      name: this.registerForm.get('jobType')!.value['name']
    };

    const governmentId: GovernmentIdDTO = {
      type: this.registerForm.get('governmentIdType')!.value,
      value: this.registerForm.get('governmentIdValue')!.value
    };

    const userForRegistration: UserForRegistrationDTO = {
      firstName: this.registerForm.get('firstName')!.value,
      lastName: this.registerForm.get('lastName')!.value,
      email: this.registerForm.get('email')!.value,
      jobType: jobType,
      incomeLevel: this.registerForm.get('incomeLevel')!.value,
      governmentId: governmentId,
      password: this.registerForm.get('password')!.value,
      confirmPassword: this.registerForm.get('confirmPassword')!.value
    };

    const register$ = this.authenticationHttpService.registerUser(userForRegistration).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Registration successful'
        })
      })
    );
    this.subscriptions.push(this.doWithLoading(register$).subscribe({
      complete: () => {
        this.router.navigate(["login"]);
      }
    }));
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
    const control = this.registerForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.registerForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isEmailInputTouchedOrDirtyAndNotValidEmail(): boolean {
    const control = this.registerForm.get('email')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('email');
  }

  isIncomeLevelInputTouchedOrDirtyAndNotValidValue(): boolean {
    const control = this.registerForm.get('incomeLevel')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('min');
  }

  isGovernmentIdValueInvalidAndTouchedOrDirty(): boolean {
    const control = this.registerForm.get('governmentIdValue')!;
    return this.isInputTouchedOrDirty(control) && (control.invalid || this.registerForm.hasError('invalidGovernmentIdValue'));
  }

  isGovernmentIdValueInvalidButNotEmptyAndTouchedOrDirty(): boolean {
    const control = this.registerForm.get('governmentIdValue')!;
    return this.isInputTouchedOrDirty(control) && this.registerForm.hasError('invalidGovernmentIdValue');
  }

  isPasswordInputTouchedOrDirtyAndTooShort(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('minlength');
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainLowerCaseLetter(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsLowerCaseLetterRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainUpperCaseLetter(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsUpperCaseLetterRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainNumber(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsNumberRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainSpecialCharacter(): boolean {
    const control = this.registerForm.get('password')!;
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsSpecialCharacterRegex.toString();
  }

  isConfirmPasswordInputInvalidAndTouchedOrDirty(): boolean {
    const control = this.registerForm.get('confirmPassword')!;
    return this.isInputTouchedOrDirty(control) && (control.invalid || this.registerForm.hasError('passwordsNotMatching'));
  }

  arePasswordAndConfirmPasswordTouchedOrDirtyAndNotMatching(): boolean {
    const passwordControl = this.registerForm.get('password')!;
    const confirmPasswordControl = this.registerForm.get('confirmPassword')!;

    return this.isInputTouchedOrDirty(passwordControl)
      && this.isInputTouchedOrDirty(confirmPasswordControl)
      && this.registerForm.hasError('passwordsNotMatching');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }

  isGovernmentIdTypeSelected(): boolean {
    return this.registerForm.get('governmentIdType')!.value != null;
  }

  getSelectedGovernmentIdType(): string {
    return this.registerForm.get('governmentIdType')!.value;
  }
}
