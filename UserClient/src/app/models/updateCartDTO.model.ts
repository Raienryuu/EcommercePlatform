import { Cart } from "./cart.model";

export interface UpdateCartDTO {
  CartGuid: string;
  Cart: Cart;
}
