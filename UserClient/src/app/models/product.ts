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

