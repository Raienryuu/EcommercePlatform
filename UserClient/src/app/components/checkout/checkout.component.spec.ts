import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutComponent } from './checkout.component';
import { CountrySelectComponent } from '../country-select/country-select.component';
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
import { provideHttpClient } from '@angular/common/http';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { DebugElement } from '@angular/core';

describe('CheckoutComponent', () => {
  let component: CheckoutComponent;
  let fixture: ComponentFixture<CheckoutComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CheckoutComponent],
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
      providers: [provideHttpClient()],
    });
    fixture = TestBed.createComponent(CheckoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should open dhl locker selector dialog', async () => {
    const dhlButton = fixture.debugElement.query(By.css('.dhl-locker'));

    dhlButton.nativeElement.click();

    const lockerSelector = new DebugElement(
      document.querySelector('app-locker-selector')!
    );
    const dhlMapIframe = lockerSelector.query(By.css('.map')).nativeElement;
    expect(dhlMapIframe).toBeInstanceOf(HTMLIFrameElement);
  });

  it('should select dhl locker delivery option', () => {
    const dhlButton = fixture.debugElement.query(By.css('.dhl-locker'));
    const dhlRadioButton = fixture.debugElement.query(By.css('[value="dhl"]'));

    dhlButton.nativeElement.click();

    component.dialogDhl.closeAll();
    fixture.detectChanges();
    const ne = dhlRadioButton.nativeElement as HTMLElement;
    expect(ne.classList.contains('mat-mdc-radio-checked')).toBeTruthy();
  });

  it('should open address editor in edit mode', async () => {
    const editAddressButton = fixture.debugElement.query(
      By.css('[fonticon=edit]'),
    ).parent;
    editAddressButton?.nativeElement.click();
    await fixture.whenStable();

    const editorElement = new DebugElement(
      document.querySelector('app-address-editor')!,
    );

    const editButton = editorElement.query(By.css('[name=edit-button]'));

    expect(editButton).withContext("didn't find the Edit button").toBeTruthy();
  });

  it('should open address editor in new mode', async () => {
    const addAddressButton = fixture.debugElement.query(
      By.css('[fonticon=add]'),
    ).parent;
    addAddressButton?.nativeElement.click();
    await fixture.whenStable();

    const editorElement = new DebugElement(
      document.querySelector('app-address-editor')!,
    );

    const addButton = editorElement.query(By.css('[name=add-button]'));

    expect(addButton).withContext("didn't find the Add button").toBeTruthy();
  });
});
