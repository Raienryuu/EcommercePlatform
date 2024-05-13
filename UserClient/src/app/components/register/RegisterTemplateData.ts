import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { NewUser } from 'src/app/models/user-registration-form';

export const confirmPasswordValidator: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  const password = control.root.get('password');
  const password2 = control.root.get('password2');

  if (
    password?.valid &&
    password2?.valid &&
    password &&
    password2 &&
    password.value !== password2.value
  ) {
    return { confirmPassword: true };
  }
  return null;
};

export const phonePrefixValidator: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  const prefix = control.root.get('phonePrefix');
  return prefix && prefix.valid ? null : { phonePrefix: true };
};

export const registerForm = new FormGroup(
  {
    login: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
    ]),
    password2: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
    ]),
    name: new FormControl('', Validators.required),
    lastname: new FormControl('', Validators.required),
    phonePrefix: new FormControl('', Validators.required),
    phoneNumber: new FormControl('', [
      phonePrefixValidator,
      Validators.required,
      Validators.pattern('^[0-9 ()+-]*$'),
    ]),
    address: new FormControl('', Validators.required),
    city: new FormControl('', Validators.required),
    zipCode: new FormControl('', Validators.required),
    country: new FormControl('', Validators.required),
  },
  { validators: confirmPasswordValidator }
);

export function ToNewUser(form: FormGroup): NewUser {
  let user: NewUser = {
    UserName: form.value.login,
    Password: form.value.password,
    Name: form.value.name,
    Lastname: form.value.lastname,
    Email: form.value.email,
    PhonePrefix: form.value.phonePrefix,
    PhoneNumber: form.value.phoneNumber,
    Address: form.value.address,
    City: form.value.city,
    ZIPCode: form.value.zipCode,
    Country: form.value.country,
  };

  return user;
}
