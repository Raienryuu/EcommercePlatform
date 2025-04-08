export interface Order {
  orderId: string;
  userId: string;
  isConfirmed: boolean;
  status: string;
  notes: string | null;
  created: Date;
  lastModified: Date;
  products: OrderProduct[];
  stripePaymentId: string;
  totalPriceInSmallestCurrencyUnit: number;
  currencyISO: string;
}

export interface CreateOrderRequest {
  notes: string | null;
  products: OrderProduct[];
  currencyISO: string;
}

export interface OrderProduct {
  productId: string;
  quantity: number;
  /** Price in currency smallest units (eg. cents for $) */
  price: number;
}
