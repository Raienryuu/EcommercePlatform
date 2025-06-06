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
  control: AbstractControl,
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
  control: AbstractControl,
): ValidationErrors | null => {
  const prefix = control.root.get('phonePrefix');
  return prefix && prefix.valid ? null : { phonePrefix: true };
};

export const RegisterFormWithValidators = new FormGroup(
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
    phoneNumber: new FormControl(
      '',
      Validators.compose([
        Validators.minLength(1),
        Validators.required,
        Validators.pattern('^[0-9 ()+-]*$'),
        phonePrefixValidator,
      ]),
    ),
    address: new FormControl('', Validators.required),
    city: new FormControl('', Validators.required),
    zipCode: new FormControl('', Validators.required),
    country: new FormControl(
      { value: '', disabled: false },
      Validators.required,
    ),
  },
  { validators: confirmPasswordValidator },
);

export function ToNewUser(form: FormGroup): NewUser {
  const user: NewUser = {
    UserName: form.value.login,
    Password: form.value.password,
    Address: {
      id: 0,
      fullName: form.value.name + ' ' + form.value.lastname,
      email: form.value.email,
      phoneNumber: form.value.phoneNumber,
      address: form.value.address,
      city: form.value.city,
      zipCode: form.value.zipCode,
      country: form.value.country,
    },
  };

  return user;
}
