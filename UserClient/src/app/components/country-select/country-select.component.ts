import {
  Component,
  EventEmitter,
  forwardRef,
  Input,
  Output,
} from '@angular/core';
import { CountriesNoPhonesSorted } from '../register/RegisterRawData';
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
  NG_VALUE_ACCESSOR, Validators,
} from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';

@Component({
    selector: 'app-country-select',
    templateUrl: './country-select.component.html',
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
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => CountrySelectComponent),
            multi: true,
        },
    ]
})
export class CountrySelectComponent implements ControlValueAccessor {
  /**
   *
   */
  constructor() {
    if (!this.validate) {
      this.countryControl = new FormControl();
    } else {
      this.country = this.countryControl.value!;
      if (this.countryControl.validator === null){
        this.countryControl = new FormControl('', [Validators.required]);
      }
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
  countryControl = new FormControl<string>('');

  selectionChanged(event: MatSelectChange) {
    this._onChange(event.value);
    this.countryChange.emit(event.value);
  }

  _onChange: (_: unknown) => void = () => true;

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  _onTouched(_: unknown) {
    this.countryControl.markAsTouched();
  }


  writeValue(obj: string): void {
    this._onChange(obj);
    this.country = obj;
    this.countryControl?.setValue(obj, { emitEvent: false });
  }

  registerOnChange(fn: (_: unknown) => void): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: (_: unknown) => void): void {
    this._onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
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
