import {
  StripeElementsOptions,
  StripePaymentElementOptions,
} from '@stripe/stripe-js';

export class StripeConfig {
  private orderId = 'pi_3POFD5C7yfdpfbDs1K4y1MF5';
  private clientSecret = '_secret_mbpshCsI0kRJtroB8J1zNQXNm';

  YOUR_CLIENT_SECRET = this.orderId + this.clientSecret;

  stripeElementsOptions: StripeElementsOptions = {
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
      type: 'auto',
      defaultCollapsed: false,
    },
  };
}
