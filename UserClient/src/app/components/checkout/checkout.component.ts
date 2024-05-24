import {
  Component,
  Input,
  ViewChild,
} from '@angular/core';
import { Product } from 'src/app/models/product';
import intlTelInput from 'intl-tel-input';
import { CountriesNoPhonesSorted } from '../register/RegisterRawData';
import { PhoneSelectComponent } from 'src/app/phone-select/phone-select.component';
import { CountrySelectComponent } from 'src/app/country-select/country-select.component';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent {
  product: Product = null!;

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

  @ViewChild(CountrySelectComponent)
  country!: CountrySelectComponent;
  @ViewChild(PhoneSelectComponent)
  phoneInput!: PhoneSelectComponent;

  selectedCountry: string = null!;


  UpdateCountry(country: string) {
    console.info("Got new country value: " + country);
    console.info(this.phoneInput.phonePrefixElement?.getNumber());
    
  }

}

@Component({
  selector: 'app-checkout2',
  templateUrl: './part2.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent2 {}
