import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserSettingsService {
  private currencySymbol = '€';

  GetCurrencySymbol(): Observable<string> {
    return of(this.currencySymbol);
  }
}
