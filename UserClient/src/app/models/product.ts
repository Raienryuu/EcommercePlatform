import { OrderProduct } from './order.model';
import { ProductCategory } from './product-category';

export interface Product {
  id: string;
  categoryId: number;
  category?: ProductCategory;
  name: string;
  description: string;
  price: number;
  quantity: number;
  concurrencyStamp?: number[];
}

export function MapToOrderProduct(this: Product): OrderProduct {
  const newOrderProduct: OrderProduct = {
    quantity: this.quantity,
    productId: this.id,
    price: this.price,
  };
  return newOrderProduct;
}
