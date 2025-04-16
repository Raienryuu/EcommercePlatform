export interface OrderProduct {
  productId: string;
  quantity: number;
  /** Price in currency smallest units (eg. cents for $) */
  price: number;
}
