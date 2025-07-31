import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountrySelectComponent } from './country-select.component';
import { NgOptimizedImage } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { Component, Signal, viewChild } from "@angular/core";
import { TestbedHarnessEnvironment } from "@angular/cdk/testing/testbed";
import { MatSelectHarness } from "@angular/material/select/testing";


describe('CountrySelectComponent', () => {
  @Component({
    selector: 'app-wrapper',
    template: `
      <app-country-select [validate]="true" [countryControl]="countryControl"/>`,
    imports: [
        CountrySelectComponent
    ]
})
  class CountrySelectWrapperComponent {
    countryControl = new FormControl<string>('', [Validators.required]);
    component = viewChild<CountrySelectComponent>(
      CountrySelectComponent);
  }

  describe('validation disabled', function() {
    let component: CountrySelectComponent;
    let fixture: ComponentFixture<CountrySelectComponent>;
    beforeEach(async () => {
      await TestBed.configureTestingModule({
        imports: [CountrySelectComponent,
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
        ]
      })
        .compileComponents();

      fixture = TestBed.createComponent(CountrySelectComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();

    });

    it('should create', () => {
      expect(component).toBeTruthy();
    });

    /*
    Tests to write:

    Something with formControl being used?
    -Validation error is displayed
     */

    it('should start with empty value', function() {
      expect(component.country).toEqual('');
    });

    it('should select first value from list', async function() {
      const [selectedCountryName, selectElement] = SelectFirstCountryFromList(fixture);

      expect(component.country).withContext('country name should match chosen one')
        .toEqual(selectedCountryName);

      expect(selectElement.innerText).withContext('DOM should show selected country')
        .toEqual(selectedCountryName);
    });
  });

  function SelectFirstCountryFromList(fixture: ComponentFixture<CountrySelectComponent | CountrySelectWrapperComponent>): [string, HTMLSelectElement] {
    const select = fixture.debugElement.query(By.css('#country')).nativeElement as HTMLSelectElement;

    select.click();
    fixture.detectChanges();

    const firstCountryOption = fixture.debugElement.query(
      x => x.name === 'mat-option');
    (firstCountryOption.nativeElement as HTMLOptionElement).click();
    fixture.detectChanges();

    return [firstCountryOption.nativeNode.innerText, select];
  }

  describe('validation enabled', function() {
    let fixture: ComponentFixture<CountrySelectWrapperComponent>;
    let component: Signal<CountrySelectComponent>;
    beforeEach(async () => {
      await TestBed.configureTestingModule({
        declarations: [],
        imports: [CountrySelectComponent, CountrySelectWrapperComponent,
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
        ]
      })
        .compileComponents();

      fixture = TestBed.createComponent(CountrySelectWrapperComponent);
      component = fixture.componentInstance.component as Signal<CountrySelectComponent>;
      fixture.detectChanges();
    });


    it('should select first value from list', async function() {
      const [selectedCountryName, selectElement] = SelectFirstCountryFromList(fixture);

      expect(component().countryControl.value).withContext('country name should match chosen one')
        .toEqual(selectedCountryName);
      expect(selectElement.innerText).withContext('DOM should show selected country')
        .toEqual(selectedCountryName);
    });

    it('should show error when closed with empty value', async function() {
      const loader = TestbedHarnessEnvironment.loader(fixture);
      const selectHarness = await loader.getHarness(MatSelectHarness);
      const select = fixture.debugElement.query(By.css('#country')).nativeElement as HTMLSelectElement;

      select.click();
      fixture.detectChanges();
      await selectHarness.close();

      expect(component().countryControl.invalid).withContext('should be invalid when empty')
        .toBeTruthy();

      const error = fixture.nativeElement.querySelector('mat-error');
      expect(error).withContext('error should be rendered')
        .toBeTruthy();
    })

  });
});


