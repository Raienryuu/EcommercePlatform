import {ComponentFixture, TestBed} from '@angular/core/testing';

import {AddressEditorComponent} from './address-editor.component';

import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {BrowserModule, By} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {NgxMatInputTelComponent} from 'ngx-mat-input-tel';
import {AppRoutingModule} from 'src/app/app-routing.module';
import {CountrySelectComponent} from '../country-select/country-select.component';
import {provideHttpClient} from '@angular/common/http';
import {MatFormFieldModule} from "@angular/material/form-field";
import {
  HttpTestingController,
  provideHttpClientTesting
} from "@angular/common/http/testing";
import {MatDialogModule} from "@angular/material/dialog";

describe('AddressEditorComponent', () => {
  let component: AddressEditorComponent;
  let fixture: ComponentFixture<AddressEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddressEditorComponent],
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
        MatCardModule,
        MatDialogModule,
        NgxMatInputTelComponent,
        CountrySelectComponent,
        MatFormFieldModule,
      ],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AddressEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();

  });

  it('with empty value form should be invalid', function () {
    expect(component.addressForm.invalid).toBeTruthy();
  });

  describe('Filled form with valid values', function () {
    let emitterSpy: jasmine.Spy;
    let httpController: HttpTestingController;
    beforeEach(() => {
      httpController = TestBed.inject(HttpTestingController);
      emitterSpy = spyOn(
        component.actionResponse, 'emit')
        .and.callThrough();

      component.addressForm.controls['address'].setValue('787 Dunbar Road');
      component.addressForm.controls['fullname'].setValue(
        'John California',
      );
      component.addressForm.controls['email'].setValue('johnyboy@mail.com');
      component.addressForm.controls['phoneNumber'].setValue(
        '+1 (213) 555-3890',
      );
      component.addressForm.controls['city'].setValue('San Jose, CA');
      component.addressForm.controls['zipcode'].setValue('95127');
      component.addressForm.controls['country'].setValue('United States');
      component.addressForm.controls['id'].setValue(333);
    });

    it('form should be valid', async function () {
      expect(component.addressForm.valid).toBeTruthy();
    });

    it('emits close window event', async function () {
      const cancelButton = fixture.debugElement.query(
        By.css('[name=cancel-button]')).nativeElement as HTMLButtonElement;

      expect(cancelButton).withContext('no button found').toBeTruthy();

      cancelButton.click();

      expect(emitterSpy).toHaveBeenCalledOnceWith(undefined);
    });

    it('emits delete address event', async function () {
      const deleteButton = fixture.debugElement.query(
        By.css('[name=delete-button]')).nativeElement as HTMLButtonElement;
      expect(deleteButton).withContext('no button found').toBeTruthy();

      const deleteSpy = spyOn(component, 'DeleteAddress')
        .and.callThrough();

      deleteButton.click();

      expect(deleteSpy).withContext('no interaction from UI received')
        .toHaveBeenCalled();
      const request = httpController.expectOne({
        url: 'https://localhost:7107/api/address/' + component.address.Id,
        method: 'DELETE',
      });
      request.flush(component.address);

      expect(emitterSpy).toHaveBeenCalledWith({Address: component.address, WasDeleted: true});
    });

    it('emits add address event', async function () {
      component.isNew = true;
      fixture.detectChanges();
      const addButton = fixture.debugElement.query(
        By.css('[name=add-button]')).nativeElement as HTMLButtonElement;
      expect(addButton).withContext('no button found').toBeTruthy();

      const addSpy = spyOn(component, 'AddAddress')
        .and.callThrough();

      addButton.click();

      expect(addSpy).withContext('no interaction from UI received')
        .toHaveBeenCalled();
      const request = httpController.expectOne({
        url: 'https://localhost:7107/api/address/',
        method: 'POST',
      });
      request.flush(component.address);

      expect(emitterSpy).toHaveBeenCalledWith({Address: component.address, WasDeleted: false});

    });

    it('emits edit address event', async function () {
      const editButton = fixture.debugElement.query(
        By.css('[name=edit-button]')).nativeElement as HTMLButtonElement;
      expect(editButton).withContext('no button found').toBeTruthy();

      const editSpy = spyOn(component, 'UpdateAddress')
        .and.callThrough();

      editButton.click();

      expect(editSpy).withContext('no interaction from UI received')
        .toHaveBeenCalled();
      const request = httpController.expectOne({
        url: 'https://localhost:7107/api/address/',
        method: 'PUT',
      });
      request.flush(component.address);

      expect(emitterSpy).toHaveBeenCalledWith({Address: component.address, WasDeleted: false});
    });

  })

});
