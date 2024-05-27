import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-phone-select',
  templateUrl: './phone-select.component.html',
})
export class PhoneSelectComponent {
  phonePrefixElement: intlTelInput.Plugin | null = null;

  @Input()
  phone: string = null!;

  @Output()
  phoneChange = new EventEmitter<string>();

  ngOnInit() {
    const input = document.querySelector('#phoneNumber');
    if (input) {
      this.phonePrefixElement = window.intlTelInput(input, {
        separateDialCode: false,
        preferredCountries: [],
        initialCountry: 'auto',
        dropdownContainer: document.body,
      });
      
      input.addEventListener('countrychange', () => {
        this.UpdatePhonePrefix(this.phonePrefixElement);
        this.phone =
          (this.phonePrefixElement?.getSelectedCountryData()?.dialCode ?? '') +
          (this.phonePrefixElement?.getNumber() ?? '');
        this.phoneChange.emit(this.phone);
      });
    }
  }

  UpdatePhonePrefix($event: any): void {
    this.phone = this.phonePrefixElement?.getSelectedCountryData()?.dialCode!;
  }
}
