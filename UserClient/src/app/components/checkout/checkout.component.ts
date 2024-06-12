import { Component, Input, ViewChild, inject, signal } from '@angular/core';
import { Product } from 'src/app/models/product';
import intlTelInput from 'intl-tel-input';
import { CountriesNoPhonesSorted } from '../register/RegisterRawData';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import {
  StripeFactoryService,
  StripePaymentElementComponent,
  injectStripe,
} from 'ngx-stripe';
import {
  PaymentIntent,
  StripeElementsOptions,
  StripeExpressCheckoutElementOptions,
  StripePaymentElementOptions,
} from '@stripe/stripe-js';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/enviroment';
import { CustomerAddress } from 'src/app/models/customer-address.model';

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
export class CheckoutComponent2 {
  stripe = this.factoryService.create(environment.stripeApiKey);
  YOUR_CLIENT_SECRET: string | null =
    'pi_3POFD5C7yfdpfbDs1K4y1MF5_secret_mbpshCsI0kRJtroB8J1zNQXNm';
  paymentIntent: any;

  constructor(private factoryService: StripeFactoryService) {}
  elementsOptions: StripeElementsOptions = {
    locale: 'en',
    clientSecret: this.YOUR_CLIENT_SECRET!,
    appearance: {
      theme: 'stripe',
      variables: {
        colorBackground: '#F0F2FF',
        colorText: '#000000de',
        fontLineHeight: '50px',

        // fontSizeBase: '20px'
      },
      labels: 'floating',
      rules: {
        '.Input': {
          lineHeight: '1.5rem',
        },
      },
    },
  };
  paymentElementOptions: StripePaymentElementOptions = {
    layout: {
      type: 'tabs',
      defaultCollapsed: false,
    },
  };

  @ViewChild(StripePaymentElementComponent)
  paymentElement!: StripePaymentElementComponent;

  private readonly fb = inject(UntypedFormBuilder);

  paymentElementForm = this.fb.group({
    email: ['support@ngx-stripe.dev', [Validators.required, Validators.email]],
  });

  paying = signal(false);
  pay() {
    if (this.paying() || this.paymentElementForm.invalid) return;
    this.paying.set(true);

    const { email } = this.paymentElementForm.getRawValue();

    this.stripe
      .confirmPayment({
        elements: this.paymentElement.elements,
        confirmParams: {
          payment_method_data: {
            billing_details: {
              email: email as string,
            },
          },
        },
        redirect: 'if_required',
      })
      .subscribe((result) => {
        this.paying.set(false);
        console.log('Result', result);
        if (result.error) {
          // Show error to your customer (e.g., insufficient funds)
          alert({ success: false, error: result.error.message });
        } else {
          // The payment has been processed!
          if (result.paymentIntent.status === 'succeeded') {
            // Show a success message to your customer
            alert({ success: true });
          }
        }
      });
  }
}

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent {
  products: Product[] = null!;
  promoCodes: String[] = null!;
  currencySymbol: String = '€';
  total = { base: 155.88, tax: 15.88, delivery: 15, total: 170.88 };

  customerAddresses: CustomerAddress[] = [
    {
      Name: 'John',
      Lastname: 'California',
      Address: '787 Dunbar Road',
      Email: 'johnyboy@mail.com',
      PhonePrefix: '+1',
      PhoneNumber: '(213) 555-3890',
      City: 'San Jose, CA',
      ZIPCode: '95127',
      Country: 'USA',
    },
    {
      Name: 'John Senior',
      Lastname: 'California',
      Address: '788B Dunbar Road',
      Email: 'oljohny@mail.com',
      PhonePrefix: '+1',
      PhoneNumber: '(213) 555-3890',
      City: 'San Jose, CA',
      ZIPCode: '95127',
      Country: 'USA',
    },
  ];
  activeSelection: Number = this.customerAddresses.length - 1;

  @ViewChild(CheckoutComponent1)
  deliveryForm!: CheckoutComponent1;

  @ViewChild(CheckoutComponent2)
  paymentSelection!: CheckoutComponent2;

  constructor(private factoryService: StripeFactoryService) {
    this.products = [
      {
        id: 5,
        categoryId: 3,
        name: 'Product E',
        description: 'Description for Product E',
        price: 12.99,
        quantity: 12,
      },
    ];
  }

  stripe = this.factoryService.create(environment.stripeApiKey);
  YOUR_CLIENT_SECRET: string | null =
    'pi_3POFD5C7yfdpfbDs1K4y1MF5_secret_mbpshCsI0kRJtroB8J1zNQXNm';
  paymentIntent: any;

  @ViewChild(StripePaymentElementComponent)
  paymentElement!: StripePaymentElementComponent;

  elementsOptions: StripeElementsOptions = {
    locale: 'en',
    clientSecret: this.YOUR_CLIENT_SECRET!,
    appearance: {
      theme: 'stripe',
      variables: {
        colorBackground: '#F0F2FF',
        colorText: '#000000de',
        fontLineHeight: '50px',

        // fontSizeBase: '20px'
      },
      labels: 'floating',
      rules: {
        '.Input': {
          lineHeight: '1.5rem',
        },
      },
    },
  };
  paymentElementOptions: StripePaymentElementOptions = {
    layout: {
      type: 'tabs',
      defaultCollapsed: false,
    },
  };

  SelectAddress(id: Number) {
    this.activeSelection = id;
    console.log(id);
  }
}
