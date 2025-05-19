import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StripePaymentComponent } from './stripe-payment.component';
import { MatButtonModule } from '@angular/material/button';
import { NgxStripeModule } from 'ngx-stripe';

describe('StripePaymentComponent', () => {
  let component: StripePaymentComponent;
  let fixture: ComponentFixture<StripePaymentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxStripeModule.forRoot(), MatButtonModule],
    }).compileComponents();

    fixture = TestBed.createComponent(StripePaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
