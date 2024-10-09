import {
  Component,
  EventEmitter,
  forwardRef,
  Input,
  Output,
} from '@angular/core';
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
  FormGroup,
  FormGroupDirective,
  FormsModule,
  NG_VALUE_ACCESSOR,
  NgForm,
  Validators,
} from '@angular/forms';
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
    if (this.validate === false){
      this.countryControl = new FormControl();
    }
  }

  countriesNoPhonesSorted: string[] = CountriesNoPhonesSorted;

  matcher = new CountryMatcher();

  @Input() country: string = null!;
  @Input() errorMessage: string = 'Please choose country';
  @Input() isDisabled: boolean = false;
  @Input() required: boolean = false;
  @Input() validate: boolean = false;

  @Output()
  countryChange = new EventEmitter<string>();
  @Output()
  formGroupChange = new EventEmitter<string>();

  @Input()
  countryControl : FormControl<any> = null!;

  selectionChanged(event: MatSelectChange) {
    this._onChange(event.value);
    this.countryChange.emit(event.value);
    
  }

  _onChange: (_: any) => void = (_) => true;
  _onTouched: any;

  writeValue(obj: any): void {
    this.country = obj as string;
  }

  registerOnChange(fn: (_: any) => void): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }
}

export class CountryMatcher implements ErrorStateMatcher {
  isErrorState(
    control: FormControl | null
  ): boolean {
    console.info(control?.value);
    return !!(
      (control?.value === undefined || control?.value === '') &&
      (control?.touched || control?.dirty)
    );
  }
}
