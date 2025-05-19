import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CartComponent } from './cart.component';
import { NgOptimizedImage } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTreeModule } from '@angular/material/tree';
import { BrowserModule, By } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxMatInputTelComponent } from 'ngx-mat-input-tel';
import { NgxStripeModule } from 'ngx-stripe';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { CountrySelectComponent } from '../country-select/country-select.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('CartComponent', () => {
  let component: CartComponent;
  let fixture: ComponentFixture<CartComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CartComponent],
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
      ],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    fixture = TestBed.createComponent(CartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('increasing quantity should change total price of a product', function () {
    const product = fixture.debugElement.query(By.css('.product'));
    expect(product).withContext('expected to find product row').toBeTruthy();
    const productQuantityPanel = product.query(By.css('.quantity-wrapper'));
    const increaseButton = productQuantityPanel.query(
      By.css('[aria-label="increase quantity"]'),
    );
    expect(increaseButton)
      .withContext('should have increment quantity button')
      .toBeTruthy();
    const previousProductTotal = parseFloat(
      (
        product.query(By.css('[name=productTotal]'))
          .nativeElement as HTMLSpanElement
      ).innerHTML,
    );
    expect(previousProductTotal)
      .withContext('single product total should be positive value')
      .toBeGreaterThan(0);
    increaseButton.nativeElement.click();
    fixture.detectChanges();

    const currentProductTotal = parseFloat(
      (
        product.query(By.css('[name=productTotal]'))
          .nativeElement as HTMLSpanElement
      ).innerHTML,
    );
    expect(currentProductTotal)
      .withContext('should be greater with bigger quanitity')
      .toBeGreaterThan(previousProductTotal);
  });

  it('decreasing quantity should change total price of a product', function () {
    const product = fixture.debugElement.query(By.css('.product'));
    expect(product).withContext('expected to find product row').toBeTruthy();
    const productQuantityPanel = product.query(By.css('.quantity-wrapper'));
    const decreaseButton = productQuantityPanel.query(
      By.css('[aria-label="decrease quantity"]'),
    );
    expect(decreaseButton)
      .withContext('should have increment quantity button')
      .toBeTruthy();
    const previousProductTotal = parseFloat(
      (
        product.query(By.css('[name=productTotal]'))
          .nativeElement as HTMLSpanElement
      ).innerHTML,
    );
    expect(previousProductTotal)
      .withContext('single product total should be positive value')
      .toBeGreaterThan(0);
    decreaseButton.nativeElement.click();
    fixture.detectChanges();

    const currentProductTotal = parseFloat(
      (
        product.query(By.css('[name=productTotal]'))
          .nativeElement as HTMLSpanElement
      ).innerHTML,
    );
    expect(currentProductTotal)
      .withContext('should be greater with bigger quanitity')
      .toBeLessThan(previousProductTotal);
  });

  it('decrease quantity button should not decrement quanitity to less than 1', async function () {
    const decreaseQuantityButton = fixture.debugElement.query(
      By.css('.mat-mdc-button-disabled'),
    );

    const product = decreaseQuantityButton.parent?.parent;
    expect(product).withContext('expected to find product row').toBeTruthy();
    expect(decreaseQuantityButton)
      .withContext('found quantity button')
      .toBeTruthy();
    const quantityInput = product?.nativeElement.querySelector(
      'input',
    ) as HTMLInputElement;

    decreaseQuantityButton.nativeElement.click();
    fixture.detectChanges();
    await fixture.whenStable();

    expect(quantityInput.value)
      .withContext("quantity shoudn't be less than 1")
      .toEqual('1');
  });

  it('should remove products with quantity lesser than 0', async function () {
    const product = fixture.debugElement.query(By.css('.product'));
    const beforeProductsRowsAmount = product.parent?.queryAll(
      By.css('.product'),
    ).length;
    expect(product).withContext('expected to find product row').toBeTruthy();
    const validateSpy = spyOn(component, 'ValidateQuantity').and.callThrough();

    const quantityInput = product.nativeElement.querySelector(
      'input',
    ) as HTMLInputElement;
    quantityInput.valueAsNumber = 0;
    quantityInput.dispatchEvent(new Event('input'));
    quantityInput.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    await fixture.whenStable();

    expect(validateSpy)
      .withContext("Validation wasn't called")
      .toHaveBeenCalled();
    const afterProductsRowsAmount =
      product.parent?.queryAll(By.css('.product')).length ?? 0;

    expect(beforeProductsRowsAmount)
      .withContext('products rows amount should decrease')
      .toBeGreaterThan(afterProductsRowsAmount);
  });
});
