import { Component, viewChild } from '@angular/core';
import { NewUser } from 'src/app/models/user-registration-form';
import { UserService } from 'src/app/services/userService/user.service';
import { FormControl, FormGroup, FormGroupDirective } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { RegisterFormWithValidators, ToNewUser } from './RegisterValidation';
import { Country } from 'ngx-mat-input-tel/lib/model/country.model';
import { NgxMatInputTelComponent } from 'ngx-mat-input-tel';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  user!: NewUser;
  confirmPassword: string;
  registerForm: FormGroup;
  passwordMatcher1: PasswordsErrorStateMatcher;
  passwordMatcher2: PasswordsErrorStateMatcher;
  phonePlaceholder: string;
  countryControlReference: FormControl;
  phoneControlReference: FormControl;

  phoneInput = viewChild<NgxMatInputTelComponent>('phone');

  constructor(private userService: UserService) {
    this.user = {
      UserName: '',
      Password: '',
      Address: {
        Id: 0,
        FullName: '',
        Email: '',
        PhoneNumber: '',
        Address: '',
        City: '',
        ZIPCode: '',
        Country: '',
      },
    };
    this.confirmPassword = '';

    // afghanistan is being selected as default,
    // with this value provided it mitigates value changed after check error
    this.phonePlaceholder = '701234567';

    this.registerForm = RegisterFormWithValidators;
    this.countryControlReference = this.registerForm.controls[
      'country'
    ] as FormControl;
    this.phoneControlReference = this.registerForm.controls[
      'phoneNumber'
    ] as FormControl;
    this.phoneControlReference.markAsDirty({
      emitEvent: true,
      onlySelf: false,
    });

    this.passwordMatcher1 = new PasswordsErrorStateMatcher();
    this.passwordMatcher2 = new PasswordsErrorStateMatcher();
  }

  Register(): void {
    if (this.registerForm.controls['phoneNumber'].invalid) {
      // forcing ngx-mat-intl to update view
      this.phoneInput()?.setDisabledState(false);
    }
    this.registerForm.markAllAsTouched();

    if (this.registerForm.valid) {
      this.user = ToNewUser(this.registerForm);

      this.userService
        .Register(this.user)
        .subscribe((result: unknown) => console.log(result));
    }
  }

  UpdatePhonePrefix($event: Country): void {
    this.phonePlaceholder = $event.placeHolder?.slice(
      $event.dialCode.length + 1,
    ) as string;
    this.registerForm.get('phonePrefix')?.setValue($event.dialCode);
    this.registerForm
      .get('phonePrefix')
      ?.updateValueAndValidity({ emitEvent: false });
  }
}

class PasswordsErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl, form: FormGroupDirective): boolean {
    const passwordControl = form.control.get('password');
    const confirmPasswordControl = form.control.get('password2');

    if (control.touched && control.invalid) {
      return true;
    }

    if (form.submitted && control.invalid) {
      return true;
    }

    if (
      passwordControl &&
      confirmPasswordControl &&
      passwordControl.valid &&
      confirmPasswordControl.valid &&
      passwordControl?.touched &&
      confirmPasswordControl?.touched
    ) {
      return passwordControl.value !== confirmPasswordControl.value;
    }
    return false;
  }
}
