
export enum SortType {
  PriceAsc = 1,
  PriceDesc = 2,
  QuantityAsc = 3
}

export interface PaginationParams {
  PageNum: number;
  PageSize: number;
  Name: string;
  MinPrice: number;
  MaxPrice: number;
  MinQuantity: number;
  Categories: number;
  Order: SortType;
  
}