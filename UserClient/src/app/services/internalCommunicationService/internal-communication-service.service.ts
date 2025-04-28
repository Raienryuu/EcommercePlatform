import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InternalCommunicationServiceService {
  userLoggedInEvent = new Subject<null>();

  NewUserLoggedInEvent() {
    this.userLoggedInEvent.next(null);
  }
}
