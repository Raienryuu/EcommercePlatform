import {
  StripeElementsOptions,
  StripePaymentElementOptions,
} from '@stripe/stripe-js';

export class StripeConfig {
  YOUR_CLIENT_SECRET =
    'pi_3POFD5C7yfdpfbDs1K4y1MF5_secret_mbpshCsI0kRJtroB8J1zNQXNm';

  stripeElementsOptions: StripeElementsOptions = {
    locale: 'en',
    clientSecret: this.YOUR_CLIENT_SECRET,
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
}
