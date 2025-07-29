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
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { DebugElement } from '@angular/core';
import { AddressEditorComponent } from '../address-editor/address-editor.component';
import { environment } from 'src/enviroment';
import { ActivatedRoute, convertToParamMap } from '@angular/router';

describe('CheckoutComponent', () => {
  let component: CheckoutComponent;
  let fixture: ComponentFixture<CheckoutComponent>;

  beforeEach((done) => {
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
        CheckoutComponent,
        AddressEditorComponent,
      ],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({
                orderId: '3259dd0b-7066-4b76-a740-692cfcbefe73',
              }),
            },
          },
        },
      ],
    });
    fixture = TestBed.createComponent(CheckoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.whenStable();
    const scripts = document.querySelectorAll('ngx-stripe-elements');
    scripts.forEach((element) => {
      element.remove();
    });
    done();
  });

  it('should create', function () {
    expect(component).toBeTruthy();
  });

  it('should open dhl locker selector dialog', function () {
    const dhlButton = fixture.debugElement.query(By.css('.dhl-locker'));

    expect(dhlButton)
      .withContext('should find the dhl map button')
      .toBeTruthy();
    dhlButton.nativeElement.click();

    const lockerSelector = new DebugElement(
      document.querySelector('app-dhl-locker')!,
    );
    const dhlMapIframe = lockerSelector.query(By.css('.map')).nativeElement;
    expect(dhlMapIframe)
      .withContext('should find map iframe')
      .toBeInstanceOf(HTMLIFrameElement);
    expect(dhlMapIframe).toBeInstanceOf(HTMLIFrameElement);
  });

  it('should select dhl locker delivery option when opening map', async function () {
    const dhlButton = fixture.debugElement.query(By.css('.dhl-locker'));
    const dhlRadioButton = fixture.debugElement.query(
      By.css('[name=dhl-select-button]'),
    );
    expect(dhlRadioButton)
      .withContext('found the dhl radio button')
      .toBeTruthy();

    dhlButton.nativeElement.click();

    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    const ne = dhlRadioButton.nativeElement as HTMLElement;
    expect(ne.classList.contains('mat-mdc-radio-checked'))
      .withContext('radio button is checked')
      .toBeTruthy();
  });

  it('should add new address', function (done) {
    const httpTestingController = TestBed.inject(HttpTestingController);
    fixture.whenStable();
    const addAddressButton = fixture.debugElement.query(
      By.css('[fonticon=add]'),
    ).parent;
    addAddressButton?.nativeElement.click();

    fixture.detectChanges();
    const editorElement = new DebugElement(
      document.querySelector('app-address-editor')!,
    );
    const editorComponent =
      editorElement.componentInstance as AddressEditorComponent;

    expect(editorComponent.isNew).toBeTruthy();
    const oldNumberOfAddresses =
      fixture.componentInstance.customerAddresses.length;

    // Setting form values
    //
    editorComponent.addressForm.controls['address'].setValue('787 Dunbar Road');
    editorComponent.addressForm.controls['fullname'].setValue(
      'John California',
    );
    editorComponent.addressForm.controls['email'].setValue('johnyboy@mail.com');
    editorComponent.addressForm.controls['phoneNumber'].setValue(
      '+1 (213) 555-3890',
    );
    editorComponent.addressForm.controls['city'].setValue('San Jose, CA');
    editorComponent.addressForm.controls['zipcode'].setValue('95127');
    editorComponent.addressForm.controls['country'].setValue('United States');
    editorComponent.addressForm.controls['id'].setValue(333);
    //

    const addButton = editorElement.query(By.css('[name=add-button]'));
    expect(addButton).withContext("didn't find the Add button").toBeTruthy();
    addButton.nativeElement.click();

    const request = httpTestingController.expectOne(
      { url: environment.apiUrl + 'v1/addresses', method: 'POST' },
      `Request to save new address in database`,
    );
    request.flush(editorComponent.address, {
      status: 200,
      statusText: 'All good',
    });

    fixture.detectChanges();

    expect(fixture.componentInstance.customerAddresses.length)
      .withContext(
        'added new address so addresses lenght should be incremented',
      )
      .toEqual(oldNumberOfAddresses + 1);
    const newAddress = component.customerAddresses.at(-1)!;

    expect(newAddress.country.length).toBeGreaterThan(0);
    expect(newAddress.fullName.length).toBeGreaterThan(0);
    component.dialogAddressEditor.closeAll();
    done();
  });

  it('should update the address', function (done) {
    const httpTestingController = TestBed.inject(HttpTestingController);
    fixture.whenStable();
    const editAddressButton = fixture.debugElement.query(
      By.css('[fonticon=edit]'),
    ).parent;
    editAddressButton?.nativeElement.click();
    fixture.detectChanges();

    const editorElement = new DebugElement(
      document.querySelector('app-address-editor')!,
    );
    const editorComponent =
      editorElement.componentInstance as AddressEditorComponent;

    expect(editorComponent.isNew).toBeFalsy();

    editorComponent.addressForm.controls['fullname'].setValue(
      'Jimmy California',
    );
    fixture.detectChanges();

    const editButton = editorElement.query(By.css('[name=edit-button]'));

    expect(editButton).withContext("didn't find the Edit button").toBeTruthy();
    editButton.nativeElement.click();

    const request = httpTestingController.expectOne(
      { url: environment.apiUrl + 'v1/addresses', method: 'PUT' },
      'Request to save updated address in database',
    );
    request.flush(editorComponent.address, {
      status: 200,
      statusText: 'All good',
    });

    fixture.detectChanges();

    expect(component.customerAddresses[0].fullName)
      .withContext('should have new name value')
      .toEqual('Jimmy California');
    expect(component.customerAddresses[0].country)
      .withContext('should keep old country value')
      .toEqual('United States');
    component.dialogAddressEditor.closeAll();
    done();
  });

  it('should delete address', function (done) {
    const httpTestingController = TestBed.inject(HttpTestingController);
    fixture.whenStable();
    const editAddressButton = fixture.debugElement.query(
      By.css('[fonticon=edit]'),
    ).parent;
    editAddressButton?.nativeElement.click();
    fixture.detectChanges();

    const editorElement = new DebugElement(
      document.querySelector('app-address-editor')!,
    );
    const editorComponent =
      editorElement.componentInstance as AddressEditorComponent;
    expect(editorComponent.isNew).toBeFalsy();

    const oldNumberOfAddresses =
      fixture.componentInstance.customerAddresses.length;

    const deleteButton = editorElement.query(By.css('[name=delete-button]'));
    expect(deleteButton)
      .withContext("didn't find the Delete button")
      .toBeTruthy();
    deleteButton.nativeElement.click();
    fixture.detectChanges();

    const request = httpTestingController.expectOne(
      {
        url:
          environment.apiUrl +
          'v1/addresses/' +
          component.customerAddresses[0].id,
        method: 'DELETE',
      },
      'Request to save new address in database',
    );
    request.flush(editorComponent.address, {
      status: 200,
      statusText: 'All good',
    });

    expect(fixture.componentInstance.customerAddresses.length)
      .withContext('deleted address so addresses lenght should be decremented')
      .toEqual(oldNumberOfAddresses - 1);
    component.dialogAddressEditor.closeAll();
    done();
  });
});
