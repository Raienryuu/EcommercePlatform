import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DeliveryMethod } from 'src/app/models/delivery-method.model';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root',
})
export class DeliveryService {
  constructor(private httpClient: HttpClient) {}

  GetAvailableDeliveries(): Observable<DeliveryMethod[]> {
    return this.httpClient.get<DeliveryMethod[]>(
      environment.apiUrl + 'v1/deliveries',
    );
  }
}
