import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, share } from 'rxjs';
import { Cart } from 'src/app/models/cart.model';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  cart = 'cart';
  cartKey = 'cartKey';

  constructor(private httpClient: HttpClient) {
    this.remoteCartId = localStorage.getItem(this.cartKey);
    this.localCart = JSON.parse(
      localStorage.getItem(this.cart) ?? '{"products":[]}',
    );
    console.info('local cart is null', this.localCart == null, this.localCart);
    if (this.remoteCartId == null && this.localCart.products.length > 0) {
      this.CreateNewCart().subscribe((res) => {
        this.remoteCartId = res;
        this.UpdateLocalStorage();
      });
      console.info('Creating new cart since localId was null');
    } else if (this.remoteCartId !== null) {
      this.GetCart().subscribe({
        next: (cart) => {
          this.localCart = cart;
          this.UpdateLocalCart();
          console.info(
            'cart is filled with ' + localStorage.getItem(this.cart),
          );
        },
      });
    }
  }

  public remoteCartId: string | null;
  public localCart: Cart;

  CreateNewCart(): Observable<string> {
    return this.httpClient.post<string>(
      environment.apiUrl + 'cart',
      this.localCart,
    );
  }

  AddToCart(productId: string, quantity = 1): Observable<string> {
    if (localStorage.getItem(this.cart) === null) {
      this.localCart = { products: [] };
    }
    this.localCart.products.push({
      id: productId,
      amount: quantity,
    });
    this.UpdateLocalCart();
    const sharedObservable = this.UpdateCart();
    sharedObservable.subscribe((id) => {
      this.remoteCartId = id;
      this.UpdateLocalCartKey();
      console.info('Heres updated cartId ' + this.remoteCartId);
    });
    return sharedObservable;
  }

  GetCart(): Observable<Cart> {
    return this.httpClient.get<Cart>(
      environment.apiUrl + `cart/${this.remoteCartId}`,
    );
  }

  RemoveFromCart(productId: number): Observable<string> {
    const productIndex = this.localCart.products.findIndex(
      (p) => p.id === productId.toString(),
    );
    this.localCart.products.splice(productIndex, 1);
    return this.UpdateCart();
  }

  ChangeProductQuantity(
    productId: string,
    newQuantity: number,
  ): Observable<string> {
    const productIndex = this.localCart.products.findIndex(
      (p) => p.id === productId,
    );
    if (newQuantity > 0) {
      this.localCart.products[productIndex].amount = newQuantity;
    } else {
      this.localCart.products.splice(productIndex, 1);
    }
    return this.UpdateCart();
  }

  /** Updates both remote and localStorage
   * @return  Shared 'Observable<string>' */
  private UpdateCart(): Observable<string> {
    this.UpdateLocalStorage();
    if (this.remoteCartId === null) {
      return this.CreateNewCart();
    }

    return this.httpClient
      .put<string>(
        environment.apiUrl + `cart/${this.remoteCartId}`,
        this.localCart,
      )
      .pipe(share());
  }

  private UpdateLocalCartKey() {
    localStorage.setItem(this.cartKey, this.remoteCartId!);
  }

  public GetLocalCart() {
    return this.localCart;
  }

  private UpdateLocalCart() {
    localStorage.setItem(this.cart, JSON.stringify(this.localCart));
  }

  UpdateLocalStorage() {
    this.UpdateLocalCart();
    this.UpdateLocalCartKey();
  }

  GetCartProductsIds(): string[] {
    const ids: string[] = [];
    this.localCart.products.forEach((product) => ids.push(product.id));
    return ids;
  }

  GetCartProductsCount(): Observable<number> {
    return this.httpClient.get<number>(
      environment.apiUrl + `cart/${this.remoteCartId}/count`,
    );
  }
}
