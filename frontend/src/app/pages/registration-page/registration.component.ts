import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { JobTypeDTO } from 'src/app/global/models/job-type-dto';
import { JobTypesHttpService } from 'src/app/global/services/job.type.http.service';

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
  passwordMinLength = 6;

  constructor(private formBuilder: FormBuilder, private jobTypesHttpService: JobTypesHttpService) { }

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({ // depricated change it
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      jobType: new FormControl('', Validators.required),
      incomeLevel: new FormControl(null, [Validators.required, Validators.min(1)]),
      governmentIdType: new FormControl(null, Validators.required),
      governmentIdValue: new FormControl('', Validators.required), // check if valid, the eye in password doesnt display correctly
      password: new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(this.passwordMinLength),
        Validators.pattern(this.containsLowerCaseLetterRegex),
        Validators.pattern(this.constainsUpperCaseLetterRegex),
        Validators.pattern(this.containsNumberRegex),
        Validators.pattern(this.constainsSpecialCharacterRegex)
      ])),
      confirmPassword: new FormControl('', [Validators.required])
    }, {
      Validators: [
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
      : {passwordAreNotTheSame: true};
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

  isPasswordInputTouchedOrDirtyAndTooShort(): boolean {
    const control = this.registerForm.get('password')!;
    console.log('Control dirty or touched: ', this.isInputTouchedOrDirty(control));
    console.log('Password too short: ', control.hasError('minLength'));
    return this.isInputTouchedOrDirty(control) && control.hasError('minLength');
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainLowerCaseLetter(): boolean {
    const control = this.registerForm.get('password')!;
    // console.log('No lower case letter: ', this.isInputTouchedOrDirty(control)
    // && control.hasError('pattern')
    // && control.errors!['pattern'].requiredPattern === this.containsLowerCaseLetterRegex.toString());
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsLowerCaseLetterRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainUpperCaseLetter(): boolean {
    const control = this.registerForm.get('password')!;
    // console.log('No upper case letter: ', this.isInputTouchedOrDirty(control)
    // && control.hasError('pattern')
    // && control.errors!['pattern'].requiredPattern === this.constainsUpperCaseLetterRegex.toString());
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsUpperCaseLetterRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainNumber(): boolean {
    const control = this.registerForm.get('password')!;
    // console.log('No number: ', this.isInputTouchedOrDirty(control)
    // && control.hasError('pattern')
    // && control.errors!['pattern'].requiredPattern === this.containsNumberRegex.toString());
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.containsNumberRegex.toString();
  }

  isPasswordInputTouchedOrDirtyAndDoesntContainSpecialCharacter(): boolean {
    const control = this.registerForm.get('password')!;
    // console.log('No special character: ', this.isInputTouchedOrDirty(control)
    // && control.hasError('pattern')
    // && control.errors!['pattern'].requiredPattern === this.constainsSpecialCharacterRegex.toString());
    return this.isInputTouchedOrDirty(control)
      && control.hasError('pattern')
      && control.errors!['pattern'].requiredPattern === this.constainsSpecialCharacterRegex.toString();
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
