import { Component } from '@angular/core';
import { NewUser } from 'src/app/models/user-registration-form';
import { CountriesNoPhonesSorted } from './RegisterRawData';
import { UserService } from 'src/app/services/userService/user.service';
import {
  FormControl,
  FormGroup,
  FormGroupDirective,
  Validators,
} from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import intlTelInput from 'intl-tel-input';
import { ToNewUser, registerForm } from './RegisterTemplateData';
import { Country } from 'ngx-mat-intl-tel-input/lib/model/country.model';

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
  phonePlaceholder: string | undefined;

  countriesNoPhonesSorted = CountriesNoPhonesSorted;

  constructor(private userService: UserService) {
    this.user = {
      UserName: '',
      Password: '',
      Name: '',
      Lastname: '',
      Email: '',
      PhonePrefix: '',
      PhoneNumber: '',
      Address: '',
      City: '',
      ZIPCode: '',
      Country: '',
    };
    this.confirmPassword = '';
    this.phonePlaceholder = '';

    this.registerForm = registerForm;
    this.passwordMatcher1 = new PasswordsErrorStateMatcher();
    this.passwordMatcher2 = new PasswordsErrorStateMatcher();
  }

  Register(): void {
    if (this.registerForm.valid) {
      this.user = ToNewUser(this.registerForm);

      this.userService
        .Register(this.user)
        .subscribe((result: any) => console.log(result));
    }
  }

  UpdatePhonePrefix($event: Country): void {
    this.registerForm
      .get('phonePrefix')
      ?.setValue($event.dialCode);
    
    this.phonePlaceholder = $event.placeHolder?.slice($event.dialCode.length + 1);

    this.registerForm.get('phoneNumber')?.updateValueAndValidity();
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
