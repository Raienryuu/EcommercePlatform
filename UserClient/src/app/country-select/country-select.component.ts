import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CountriesNoPhonesSorted } from '../components/register/RegisterRawData';
import {
  MatError,
  MatFormField,
  MatLabel,
  MatOption,
  MatSelect,
  MatSelectChange,
} from '@angular/material/select';
import {
  ControlValueAccessor,
  FormControl,
  FormsModule,
} from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';

@Component({
  selector: 'app-country-select',
  templateUrl: './country-select.component.html',
  standalone: true,
  imports: [
    MatFormField,
    MatLabel,
    MatError,
    MatOption,
    MatSelect,
    FormsModule,
    FormsModule,
    ReactiveFormsModule,
  ],
})
export class CountrySelectComponent implements ControlValueAccessor {
  /**
   *
   */
  constructor() {
    if (this.validate === false) {
      this.countryControl = new FormControl();
    }
  }

  countriesNoPhonesSorted: string[] = CountriesNoPhonesSorted;

  matcher = new CountryMatcher();

  @Input() country = '';
  @Input() errorMessage = 'Country is required';
  @Input() isDisabled = false;
  @Input() required = false;
  @Input() validate = false;

  @Output()
  countryChange = new EventEmitter<string>();

  @Input()
  countryControl: FormControl<string> = null!;

  selectionChanged(event: MatSelectChange) {
    this._onChange(event.value);
    this.countryChange.emit(event.value);
  }

  _onChange: (_: unknown) => void = () => true;
  _onTouched: unknown;

  writeValue(obj: unknown): void {
    this.country = obj as string;
  }

  registerOnChange(fn: (_: unknown) => void): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: unknown): void {
    this._onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }
}

export class CountryMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null): boolean {
    return !!(
      (control?.value === null ||
        control?.value === '' ||
        control?.value === undefined) &&
      (control?.touched || control?.dirty)
    );
  }
}
