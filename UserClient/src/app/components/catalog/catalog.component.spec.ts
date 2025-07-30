import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsComponent } from './catalog.component';
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
import { environment } from 'src/enviroment';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('CatalogComponent', () => {
  let component: ProductsComponent;
  let fixture: ComponentFixture<ProductsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
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
        ProductsComponent,
      ],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    fixture = TestBed.createComponent(ProductsComponent);
    component = fixture.componentInstance;
    environment.sampleData = true;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should use test environment', () => {
    expect(environment.sampleData)
      .withContext('not using sample data')
      .toBeTruthy();
  });

  it('should display at least 1 product', () => {
    const products = fixture.debugElement.queryAll(By.css('.product-details'));

    expect(products.length)
      .withContext('there are no products displayed')
      .toBeGreaterThan(0);
  });

  it('should call AddToCart on click', () => {
    const addToCartBtn = fixture.debugElement.query(By.css('.product-btn'))
      .nativeElement as HTMLElement;
    const cartServiceSpy = spyOn(component, 'AddToCart');

    addToCartBtn?.click();
    fixture.detectChanges();

    expect(cartServiceSpy).toHaveBeenCalled();
  });

  //filters tests
  it('should create buttons to remove min price filter', () => {
    const updateFilters = spyOn(
      component,
      'RefreshFilterDelay',
    ).and.callThrough();
    const priceFilters = fixture.debugElement.query(By.css('.price-filter'));
    expect(priceFilters)
      .withContext("couldn't find price filter div")
      .toBeTruthy();
    const priceInput = priceFilters.query((p) => p.name == 'input')
      .nativeElement as HTMLInputElement;
    priceInput.valueAsNumber = 5;
    priceInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    expect(updateFilters)
      .withContext(
        'method to update filters was not called after inputing filter value',
      )
      .toHaveBeenCalled();

    const quickFilterPreviewButton = fixture.debugElement.query(
      By.css('.applied-filter'),
    ).nativeElement as HTMLButtonElement;
    expect(quickFilterPreviewButton)
      .withContext("didn't find the quick filter removal button")
      .toBeTruthy();
    const removeFilterSpy = spyOn(
      component,
      'RemoveMinPriceFilter',
    ).and.callThrough();
    quickFilterPreviewButton.click();
    fixture.detectChanges();
    expect(removeFilterSpy)
      .withContext('method to remove minimum price filter have not been called')
      .toHaveBeenCalled();

    const buttonAfterRemoval = fixture.debugElement.query(
      By.css('.applied-filter'),
    );
    expect(buttonAfterRemoval)
      .withContext('quickfilter button still exists')
      .toBeNull();
  });
});
