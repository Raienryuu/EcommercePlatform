import { Component, EventEmitter, Input, Output, createComponent } from '@angular/core';
import { Product } from 'src/app/models/product';
import * as intlTelInput from 'intl-tel-input';
import { CountriesNoPhonesSorted } from '../register/RegisterRawData';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})

export class CheckoutComponent {

  product: Product = null!;

  isPart1Ready: boolean = false;


  constructor() {
    this.product = { id: 5, categoryId: 3, name: "Product E", description: "Description for Product E", price: 12.99, quantity: 12 };
  }

}

@Component({
  selector: 'app-checkout1',
  templateUrl: './part1.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent1 {
  phonePrefixElement: intlTelInput.Plugin | null = null;

  @Input()
  product: Product = null!;


  phone: string = null!;
  countriesNoPhonesSorted: string[] = CountriesNoPhonesSorted;

  @Output()
  rendered: EventEmitter<boolean> = new EventEmitter<boolean>();

  ngAfterViewInit(): void {
    this.rendered.emit(true);
    
    const input = document.querySelector('#phoneNumber');
    if (input) {
      this.phonePrefixElement = window.intlTelInput(input, {
        separateDialCode: false,
        preferredCountries: [],
        initialCountry: 'auto',
        dropdownContainer: document.body
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

@Component({
  selector: 'app-checkout2',
  templateUrl: './part2.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent2 {

}