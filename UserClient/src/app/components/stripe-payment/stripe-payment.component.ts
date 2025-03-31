import { NgIf } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { StripeError } from '@stripe/stripe-js';
import {
  NgxStripeModule,
  StripeElementsDirective,
  StripeFactoryService,
  StripeInstance,
  StripePaymentElementComponent,
} from 'ngx-stripe';
import { StripeConfig } from 'src/app/stripe-settings';
import { environment } from 'src/enviroment';

@Component({
  selector: 'app-stripe-payment',
  standalone: true,
  imports: [NgxStripeModule, NgIf],
  template: `
    <div *ngIf="clientSecret !== undefined">
      <ngx-stripe-elements
        [stripe]="stripe"
        [elementsOptions]="elementsOptions"
      >
        <ngx-stripe-payment [options]="paymentElementOptions" />
      </ngx-stripe-elements>
    </div>
  `,
  styles: `
    .paymentStripe {
      padding: 1.5vh;
      background-color: #eaebf6;
      font-size: inherit;
      line-height: 1.2em;
    }
  `,
})
export class StripePaymentComponent implements OnChanges {
  @Input({ required: true })
  makePaymentEmitter: EventEmitter<null> = new EventEmitter<null>();

  @Input({ required: true })
  id: string | undefined;

  @Input({ required: true })
  set clientSecret(clientSecret: string | undefined) {
    this._clientSecret = clientSecret;
    this.elementsOptions.clientSecret = clientSecret;
  }
  get clientSecret(): string | undefined {
    return this._clientSecret;
  }

  private _clientSecret: string | undefined;

  constructor(private stripeFactoryService: StripeFactoryService) {
    this.makePaymentEmitter.subscribe(() => this.MakeStripePayment());
  }

  ngOnChanges(changes: SimpleChanges): void {
    const secretChanges = changes['clientSecret'];
    if (secretChanges.currentValue != secretChanges.previousValue) {
      this.elementsOptions.clientSecret = secretChanges.currentValue;
    }
  }

  stripe: StripeInstance = this.stripeFactoryService.create(
    environment.stripeApiKey,
  );

  stripeConfig = new StripeConfig();

  @ViewChild(StripePaymentElementComponent)
  private paymentElement!: StripePaymentElementComponent;

  @ViewChild(StripeElementsDirective)
  private formPaymentElement!: StripeElementsDirective;

  elementsOptions = this.stripeConfig.stripeElementsOptions;
  paymentElementOptions = this.stripeConfig.paymentElementOptions;

  UpdateClientSecret(newClientSecret: string) {
    this.clientSecret = newClientSecret;
    this.elementsOptions.clientSecret = newClientSecret;
  }

  public MakeStripePayment() {
    if (this.id == undefined) {
      throw new Error('Order Id is required');
    }

    if (this.clientSecret == undefined) {
      throw new Error('Client secret is required');
    }

    this.formPaymentElement.submit().subscribe({
      next: () => {
        this.stripe
          .confirmPayment({
            elements: this.paymentElement.elements,
            confirmParams: {
              return_url: 'http://localhost:4200/myorders/' + this.id,
            },
            clientSecret: this.clientSecret!,
          })
          .subscribe((result) => console.log(result));
      },
      error: (error: StripeError | undefined) => {
        throw new Error(error?.message);
      },
    });
  }
}
