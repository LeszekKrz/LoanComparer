import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { JobType } from 'src/app/core/models/job-type';
import { JobTypeDTO } from 'src/app/core/models/job-type-dto';
import { JobTypesHttpService } from 'src/app/core/services/job.type.http.service';

@Component({
  selector: 'app-create-inquiry',
  templateUrl: './create-inquiry.component.html',
  styleUrls: ['./create-inquiry.component.scss']
})
export class CreateInquiryComponent implements OnInit, OnDestroy {
  createInquiryForm!: FormGroup;
  isProgressSpinnerVisible = false;
  subscriptions: Subscription[] = [];
  governmentIdTypes = ['PESEL', 'ID Number', 'Passport Number'];
  jobTypes: JobType[] = [];

  constructor(private formBuilder: FormBuilder,
    private jobTypesHttpService: JobTypesHttpService) { }

  ngOnInit(): void {
    // jesli zalogowany to pobierz dane

    this.createInquiryForm = this.formBuilder.group({
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      jobType: new FormControl('', Validators.required),
      incomeLevel: new FormControl(null, [Validators.required, Validators.min(1)]),
      governmentIdType: new FormControl(null, Validators.required),
      governmentIdValue: new FormControl('', Validators.required),
      loanValue: new FormControl(null, [Validators.required, Validators.min(1)]),
      numberOfInstallments: new FormControl(null, [Validators.required, Validators.min(1)]),
    }, {
      validators: [
        this.governmentIdValueIsValid,
      ],
    });

    const getJobTypes$ = this.jobTypesHttpService.getJobTypes().pipe(
      tap((jobTypes: JobTypeDTO[]) => {
        this.jobTypes = jobTypes.map(jobType => ({name: jobType.name}));
      })
    );
    this.subscriptions.push(this.doWithLoading(getJobTypes$).subscribe());
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
    // to do submit a form
  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.createInquiryForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.createInquiryForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isEmailInputTouchedOrDirtyAndNotValidEmail(): boolean {
    const control = this.createInquiryForm.get('email')!;
    return this.isInputTouchedOrDirty(control) && control.hasError('email');
  }

  isNumberInputTouchedOrDirtyAndTooSmall(numberInputName: string): boolean {
    const control = this.createInquiryForm.get(numberInputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('min');
  }

  isGovernmentIdTypeSelected(): boolean {
    return this.createInquiryForm.get('governmentIdType')!.value != null;
  }

  getSelectedGovernmentIdType(): string {
    return this.createInquiryForm.get('governmentIdType')!.value;
  }

  isGovernmentIdValueInvalidAndTouchedOrDirty(): boolean {
    const control = this.createInquiryForm.get('governmentIdValue')!;
    return this.isInputTouchedOrDirty(control) && (control.invalid || this.createInquiryForm.hasError('invalidGovernmentIdValue'));
  }

  isGovernmentIdValueInvalidButNotEmptyAndTouchedOrDirty(): boolean {
    const control = this.createInquiryForm.get('governmentIdValue')!;
    return this.isInputTouchedOrDirty(control) && this.createInquiryForm.hasError('invalidGovernmentIdValue');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }
}
