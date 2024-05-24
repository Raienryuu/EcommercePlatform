import { Component } from '@angular/core';

@Component({
  selector: 'app-phone-select',
  templateUrl: './phone-select.component.html',
})
export class PhoneSelectComponent {
  phonePrefixElement: intlTelInput.Plugin | null = null;
  phone: string = null!;

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
      });
    }
  }

  UpdatePhonePrefix($event: any): void {
    this.phone = this.phonePrefixElement?.getSelectedCountryData()?.dialCode!;
  }
}
