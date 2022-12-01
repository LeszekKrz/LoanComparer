import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateInquiryComponent } from './create-inquiry.component';

describe('CreateInquiryComponent', () => {
  let component: CreateInquiryComponent;
  let fixture: ComponentFixture<CreateInquiryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateInquiryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateInquiryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
