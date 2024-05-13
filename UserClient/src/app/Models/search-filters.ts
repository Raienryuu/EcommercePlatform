import 'currency.js';
import * as currency from 'currency.js';

export enum SortType {
  PriceAsc = 1,
  PriceDesc = 2,
  QuantityAsc = 3
}

export interface SearchFilters {

  Name: String;
  MinPrice: currency;
  MaxPrice: currency;
  MinQuantity: Number;
  Categories: Number;
  Order: SortType;
}