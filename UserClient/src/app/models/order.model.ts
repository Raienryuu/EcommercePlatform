import { OrderDelivery } from './order-delivery.model';
import { OrderProduct } from './order-product.model';

export interface Order {
  orderId: string;
  userId: string;
  isConfirmed: boolean;
  status: number;
  notes: string | null;
  created: Date;
  lastModified: Date;
  products: OrderProduct[];
  stripePaymentId: string;
  totalPriceInSmallestCurrencyUnit: number;
  currencyISO: string;
  delivery: OrderDelivery | null;
}
