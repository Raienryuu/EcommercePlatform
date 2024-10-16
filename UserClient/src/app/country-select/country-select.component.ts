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
import { ControlValueAccessor, FormControl, FormsModule } from '@angular/forms';
import { SuffixTransform } from '../pipes/phone-suffix-transform.pipe';
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
    SuffixTransform,
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

  @Input() country: string = null!;
  @Input() errorMessage = 'Please choose country';
  @Input() isDisabled = false;
  @Input() required = false;
  @Input() validate = false;

  @Output()
  countryChange = new EventEmitter<string>();


  @Input()
  countryControl: FormControl<unknown> = null!;

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
      (control?.value === undefined || control?.value === '') &&
      (control?.touched || control?.dirty)
    );
  }
}
