import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-create-inquiry',
  templateUrl: './create-inquiry.component.html',
  styleUrls: ['./create-inquiry.component.scss']
})
export class CreateInquiryComponent implements OnInit {
  createInquiryForm!: FormGroup;

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.createInquiryForm = this.formBuilder.group({
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
    });
  }

  onSubmit(): void {

  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.createInquiryForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isEmailInputTouchedOrDirtyAndEmpty(): boolean {
    const control = this.createInquiryForm.get('email')!;
    console.log("email empty")
    console.log(this.isInputTouchedOrDirty(control) && control.hasError('required'));
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isEmailInputTouchedOrDirtyAndNotValidEmail(): boolean {
    const control = this.createInquiryForm.get('email')!;
    console.log("Email valid? " + this.isInputTouchedOrDirty(control) && control.hasError('email'));
    return this.isInputTouchedOrDirty(control) && control.hasError('email');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }
}
