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
  elementsOptions!: StripeElementsOptions;
  paymentIntent: any;

  constructor(private factoryService: StripeFactoryService) {
    this.elementsOptions = {
      locale: 'en',
      clientSecret: this.YOUR_CLIENT_SECRET!,
      appearance: {
        theme: 'stripe',
        variables: {          
          colorBackground: '#F0F2FF',
          colorText: '#000000de',
          fontLineHeight: '50px'
          // fontSizeBase: '20px'
        },
        labels: 'floating',
        rules: {
          '.Input' : {
            'lineHeight': '1.5rem',
          }
        }
      },
    };
  }

  paymentElementOptions: StripePaymentElementOptions = {
    layout: {
      type: 'tabs',
      defaultCollapsed: false,
    }
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
