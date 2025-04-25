import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root',
})
export class UserSettingsService {
  /**
   *
   */
  constructor(private httpClient: HttpClient) {}
  private currencySymbol = '€';
  private currencyISO = 'eur';

  GetCurrencySymbol(): Observable<string> {
    return of(this.currencySymbol);
  }

  GetCurrencyISO(): string {
    return this.currencyISO;
  }

  GetUserName(): Observable<string> {
    return this.httpClient.get<string>(environment.apiUrl + 'v1/user/username');
  }
}
