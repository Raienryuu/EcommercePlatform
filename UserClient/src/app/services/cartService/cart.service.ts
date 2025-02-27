import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, share } from 'rxjs';
import { Cart } from 'src/app/models/cart.model';
import { UpdateCartDTO } from 'src/app/models/updateCartDTO.model';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  cart = "cart";
  cartKey = "cartKey";

  constructor(private httpClient: HttpClient) {
    if (this.remoteCartId === null && this.localCart.Products.length > 0) {
      this.CreateNewCart().subscribe(res => {
        this.remoteCartId = res;
        this.UpdateLocalStorage();
      });
      console.info("Creating new cart since localId was null");
    }
    console.info("cart is filled with " + localStorage.getItem(this.cart));
  }

  public remoteCartId: string = localStorage.getItem(this.cartKey)!;
  public localCart: Cart = JSON.parse(localStorage.getItem(this.cart) ?? "{\"Products\":[]}");

  CreateNewCart(): Observable<string> {
    const observable = this.httpClient.post<string>(environment.apiUrl + 'cart', this.localCart);
    observable.subscribe(cartId => {
      this.remoteCartId = cartId;
      this.UpdateLocalCartKey();
    })
    return observable;
  }

  AddToCart(productId: number, quantity = 1): Observable<string> {
    if (localStorage.getItem(this.cart) === null) {
      this.localCart = { Products: [] };
    }
    this.localCart.Products.push({ id: productId.toString(), amount: quantity });
    this.UpdateLocalCart();
    const sharedObservable = this.UpdateCart();
    sharedObservable.subscribe(id => {
      this.remoteCartId = id;
      this.UpdateLocalCartKey();
      console.info("Heres updated cartId " + this.remoteCartId);
    });
    return sharedObservable;
  }

  GetCart(): Observable<Cart> {
    return this.httpClient.get<Cart>(environment.apiUrl + `cart/${this.remoteCartId}`);
  }

  RemoveFromCart(productId: number): Observable<string> {
    const productIndex = this.localCart.Products.findIndex(p => p.id === productId.toString());
    this.localCart.Products.splice(productIndex, 1);
    return this.UpdateCart();
  }

  ChangeProductQuantity(productId: number, newQuantity: number): Observable<string> {
    const productIndex = this.localCart.Products.findIndex(p => p.id === productId.toString());
    if (newQuantity > 0) {
      this.localCart.Products[productIndex].amount = newQuantity;
    }
    else {
      this.localCart.Products.splice(productIndex);
    }
    return this.UpdateCart();
  }

  /** Updates both remote and localStorage
   * Return shared observable*/
  private UpdateCart(): Observable<string> {
    this.UpdateLocalStorage();
    if (this.remoteCartId === null) {
      return this.CreateNewCart();
    }
    const newCartState: UpdateCartDTO = { CartGuid: this.remoteCartId, Cart: this.localCart }
    return this.httpClient.put<string>(environment.apiUrl + 'cart/updateCart', newCartState).pipe(share());
  }

  private UpdateLocalCartKey() {
    localStorage.setItem(this.cartKey, this.remoteCartId);
  }

  private UpdateLocalCart() {
    localStorage.setItem(this.cart, JSON.stringify(this.localCart));
  }

  UpdateLocalStorage() {
    this.UpdateLocalCart();
    this.UpdateLocalCartKey();
  }

  GetCartProductsIds(): number[] {
    const ids: number[] = [];
    this.localCart.Products.forEach(product => ids.push(
      parseInt(product.id)));
    return ids;
  }
}

