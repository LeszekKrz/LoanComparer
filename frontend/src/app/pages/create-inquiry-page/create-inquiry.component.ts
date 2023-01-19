import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { JobType } from 'src/app/core/models/job-type';
import { JobTypeDTO } from 'src/app/core/models/job-type-dto';
import { JobTypesHttpService } from 'src/app/core/services/job.type.http.service';
import { CreateInquiryResponse } from './models/create-inquiry-response';
import { InquiryDTO } from './models/inquiry-dto';
import { UserInfoDTO } from './models/user-info-dto';
import { InquiryHttpServiceService } from './services/inquiry.http.service.service';

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
  isUserAuthenticated!: boolean;

  constructor(private formBuilder: FormBuilder,
    private jobTypesHttpService: JobTypesHttpService,
    private inquiryHttpService: InquiryHttpServiceService,
    private messageService: MessageService,
    private router: Router,
    private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
    this.isUserAuthenticated = this.authenticationService.isUserAuthenticated();

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

    if (this.isUserAuthenticated) {
      const getUserInfo$ = this.inquiryHttpService.getUserInfo().pipe(
        tap((userInfoDTO: UserInfoDTO) => {
          this.createInquiryForm.setValue({
            firstName: userInfoDTO.firstName,
            lastName: userInfoDTO.lastName,
            email: userInfoDTO.email,
            jobType: userInfoDTO.jobType != null
              ? {name: userInfoDTO.jobType}
              : null,
            incomeLevel: userInfoDTO.incomeLevel,
            governmentIdType: userInfoDTO.governmentIdType,
            governmentIdValue: userInfoDTO.governmentIdValue,
            loanValue: null,
            numberOfInstallments: null,
          });
        })
      );
      this.subscriptions.push(this.doWithLoading(getUserInfo$).subscribe());
    }
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
    if (this.createInquiryForm.invalid) {
      this.createInquiryForm.markAllAsTouched();
      return;
    }

    const createInquiryDTO: InquiryDTO = {
      amountRequested: this.createInquiryForm.get('loanValue')!.value,
      numberOfInstallments: this.createInquiryForm.get('numberOfInstallments')!.value,
      personalData: {
        notificationEmail: this.createInquiryForm.get('email')!.value,
        firstName: this.createInquiryForm.get('firstName')!.value,
        lastName: this.createInquiryForm.get('lastName')!.value,
        birthDate: null,
      },
      governmentId: {
        type: this.createInquiryForm.get('governmentIdType')!.value,
        value: this.createInquiryForm.get('governmentIdValue')!.value,
      },
      jobDetails: {
        jobName: this.createInquiryForm.get('jobType')!.value.name,
        incomeLevel: this.createInquiryForm.get('incomeLevel')!.value,
        description: null,
        startDate: null,
        endDate: null,
      },
    };

    const createInquiry$ = this.inquiryHttpService.createInquiry(createInquiryDTO).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Inquiry created'
        })
      }),
    );

    this.subscriptions.push(this.doWithLoading(createInquiry$).subscribe({
      next: (createInquiryResponse: CreateInquiryResponse) => {
        this.router.navigate(['choose-offer', {inquiryId: createInquiryResponse.inquiryId}]);
      }
    }));
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
