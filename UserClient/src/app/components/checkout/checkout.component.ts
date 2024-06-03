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
  StripePaymentElementOptions,
} from '@stripe/stripe-js';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/enviroment';

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
    'pi_3PNYVxC7yfdpfbDs1hlzG3Gv_secret_tOHGHXP3yFaMyUSCaXbAoCNc6';
  elementsOptions!: StripeElementsOptions;
  paymentIntent: any;

  constructor(
    private factoryService: StripeFactoryService,
    private httpClient: HttpClient
  ) {
    this.elementsOptions = {
      locale: 'en',
      clientSecret: this.YOUR_CLIENT_SECRET!,
      appearance: {
        theme: 'flat',
        variables: {
          colorBackground: 'black',
        },
      },
    };
  }

  paymentElementOptions: StripePaymentElementOptions = {
    layout: {
      type: 'tabs',
      defaultCollapsed: false,
      radios: false,
      spacedAccordionItems: false,
    },
  };

  @ViewChild(StripePaymentElementComponent)
  paymentElement!: StripePaymentElementComponent;

  private readonly fb = inject(UntypedFormBuilder);

  paymentElementForm = this.fb.group({
    name: ['John doe', [Validators.required]],
    email: ['support@ngx-stripe.dev', [Validators.required]],
    address: [''],
    zipcode: [''],
    city: [''],
    amount: ['2000', [Validators.required]],
  });

  paying = signal(false);
  pay() {
    console.debug(this.paymentElementForm);
    if (this.paying() || this.paymentElementForm.invalid) return;
    console.debug('Pay started!');
    this.paying.set(true);

    const { name, email, address, zipcode, city } =
      this.paymentElementForm.getRawValue();

    this.stripe
      .confirmPayment({
        elements: this.paymentElement.elements,
        confirmParams: {
          payment_method_data: {
            billing_details: {
              name: name as string,
              email: email as string,
              address: {
                line1: address as string,
                postal_code: zipcode as string,
                city: city as string,
              },
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
