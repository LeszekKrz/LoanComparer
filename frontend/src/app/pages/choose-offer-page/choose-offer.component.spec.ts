import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChooseOfferComponent } from './choose-offer.component';

describe('ChooseOfferComponent', () => {
  let component: ChooseOfferComponent;
  let fixture: ComponentFixture<ChooseOfferComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChooseOfferComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChooseOfferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
