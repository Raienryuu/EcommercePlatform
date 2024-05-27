import { Component, Input, ViewChild } from '@angular/core';
import { Product } from 'src/app/models/product';
import intlTelInput from 'intl-tel-input';
import { CountriesNoPhonesSorted } from '../register/RegisterRawData';

@Component({
  selector: 'app-checkout1',
  templateUrl: './part1.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent1 {
  phonePrefixElement: intlTelInput.Plugin | null = null;
  phone: string = null!;

  @Input()
  product: Product = null!;
  readyToPopulate: boolean = false;

  countriesNoPhonesSorted: string[] = CountriesNoPhonesSorted;

  selectedCountry: string = null!;
  phoneNumber: string = null!;

  UpdateCountry(country: string) {
    console.info('Got new country value: ' + country);
  }
}

@Component({
  selector: 'app-checkout2',
  templateUrl: './part2.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent2 {}

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent {
  product: Product = null!;

  @ViewChild(CheckoutComponent1)
  deliveryForm!: CheckoutComponent1;

  @ViewChild(CheckoutComponent2)
  paymentSelection!: CheckoutComponent2;

  constructor() {
    this.product = {
      id: 5,
      categoryId: 3,
      name: 'Product E',
      description: 'Description for Product E',
      price: 12.99,
      quantity: 12,
    };
  }
}
