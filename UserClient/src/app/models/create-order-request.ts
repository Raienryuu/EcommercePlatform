import { OrderProduct } from './order-product.model';

export interface CreateOrderRequest {
  notes: string | null;
  products: OrderProduct[];
  currencyISO: string;
}
