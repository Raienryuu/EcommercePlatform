import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterComponent } from './register.component';
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
import { UserService } from 'src/app/services/userService/user.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
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
        RegisterComponent,
    ],
    providers: [provideHttpClient()],
});
    fixture = TestBed.createComponent(RegisterComponent);
    const userService = TestBed.inject(UserService);
    userServiceSpy = spyOnAllFunctions(userService, true);
    component = fixture.componentInstance;
    component.registerForm.reset();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('shouldnt call register when form is empty', async () => {
    const registerButton: HTMLButtonElement = fixture.debugElement.query(
      By.css('[type=submit]'),
    ).nativeElement;
    registerButton.click();
    await fixture.whenStable();
    expect(component.registerForm.invalid).toBeTruthy();
    expect(userServiceSpy.Register).not.toHaveBeenCalled();
  });

  it('should call register with valid form data', () => {
    component.registerForm.controls['login'].setValue('user123');
    component.registerForm.controls['email'].setValue('user222@mail.com');
    component.registerForm.controls['password'].setValue('pa$$word');
    component.registerForm.controls['password2'].setValue('pa$$word');
    component.registerForm.controls['name'].setValue('Tommy');
    component.registerForm.controls['lastname'].setValue('Test');
    component.registerForm.controls['phonePrefix'].setValue('+48');
    component.registerForm.controls['phoneNumber'].setValue('+48999222111');
    component.registerForm.controls['address'].setValue('Milky Way 2');
    component.registerForm.controls['city'].setValue('Andromeda');
    component.registerForm.controls['zipCode'].setValue('001');
    component.registerForm.controls['country'].setValue('United States');

    const registerButton: HTMLButtonElement = fixture.debugElement.query(
      By.css('[type=submit]'),
    ).nativeElement;
    fixture.detectChanges();
    registerButton.click();
    expect(userServiceSpy.Register).toHaveBeenCalled();
  });

  it('password form controls should be invalid with not matching passwords', () => {
    component.registerForm.controls['password'].setValue('pa$$word');
    component.registerForm.controls['password2'].setValue('password');

    expect(component.registerForm.hasError('confirmPassword')).toBeTruthy();
  });

  it('password form controls should be valid with matching valid passwords', () => {
    component.registerForm.controls['password'].setValue('password');
    component.registerForm.controls['password2'].setValue('password');

    expect(component.registerForm.hasError('confirmPassword')).toBeFalsy();
  });

});
