import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderDetailsComponent } from './order-details.component';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { NgxMatInputTelComponent } from 'ngx-mat-input-tel';
import { CountrySelectComponent } from '../country-select/country-select.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatCardModule } from '@angular/material/card';
import { NgxStripeModule } from 'ngx-stripe';
import { MatStepperModule } from '@angular/material/stepper';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatTreeModule } from '@angular/material/tree';
import { NgOptimizedImage } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { BrowserModule } from '@angular/platform-browser';
import { OrderStatusTransformPipe } from 'src/app/pipes/order-status-transform.pipe';

describe('OrderDetailsComponent', () => {
  let component: OrderDetailsComponent;
  let fixture: ComponentFixture<OrderDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OrderDetailsComponent],
      imports: [
        BrowserModule,
        FormsModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        MatButtonModule,
        MatInputModule,
        MatGridListModule,
        MatSelectModule,
        ReactiveFormsModule,
        MatIconModule,
        MatSidenavModule,
        NgOptimizedImage,
        MatTreeModule,
        MatListModule,
        MatProgressSpinnerModule,
        MatPaginatorModule,
        MatStepperModule,
        NgxStripeModule.forRoot(),
        MatCardModule,
        MatRadioModule,
        MatCheckboxModule,
        MatDialogModule,
        NgxMatInputTelComponent,
        CountrySelectComponent,
        OrderStatusTransformPipe,
      ],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({
                id: '08465a47-c9c7-459b-bdc5-d98849cacb2e',
              }),
            },
          },
        },
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(OrderDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
