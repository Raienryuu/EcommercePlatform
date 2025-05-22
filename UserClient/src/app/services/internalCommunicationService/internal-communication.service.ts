import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InternalCommunicationService {
  userLoggedInEvent = new Subject<null>();
  newProductInCartEvent = new Subject<number>();

  NewUserLoggedInEvent() {
    this.userLoggedInEvent.next(null);
  }

  NewProductInCartEvent(cartItems: number) {
    this.newProductInCartEvent.next(cartItems);
  }
}
