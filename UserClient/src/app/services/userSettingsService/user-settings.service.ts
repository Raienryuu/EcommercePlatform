import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserSettingsService {
  private currencySymbol = '€';
  private currencyISO = 'eur';

  GetCurrencySymbol(): Observable<string> {
    return of(this.currencySymbol);
  }

  GetCurrencyISO(): string {
    return this.currencyISO;
  }
}
