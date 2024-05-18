import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserSettingsService {

  constructor() { }

  private currencySymbol: string = '€';

  GetCurrencySymbol(): Observable<string> {
    return of(this.currencySymbol);
  }
}
