import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateOrderRequest } from 'src/app/models/create-order-request';
import { Order } from 'src/app/models/order.model';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(private httpClient: HttpClient) {}

  CreateNewOrder(order: CreateOrderRequest): Observable<Order> {
    return this.httpClient.post<Order>(environment.apiUrl + 'v1/orders', order);
  }

  GetOrder(orderId: string): Observable<Order> {
    return this.httpClient.get<Order>(
      environment.apiUrl + `v1/orders/${orderId}`,
    );
  }

  CreatePaymentIntent(orderId: string): Observable<HttpResponse<string>> {
    return this.httpClient.post<string>(
      environment.apiUrl + 'v1/payments/' + orderId,
      null,
      { observe: 'response' },
    );
  }

  GetPaymentStatus(orderId: string) {
    return this.httpClient.get<string>(
      environment.apiUrl + `v1/payments/${orderId}`,
    );
  }
}
