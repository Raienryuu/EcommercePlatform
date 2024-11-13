import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CustomerAddress } from 'src/app/models/customer-address.model';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root',
})
export class AddressService {

  API: string = environment.apiUrl + 'address/';
  constructor(private httpClient: HttpClient) {}

  GetAddresses(): Observable<CustomerAddress[]> {
    return this.httpClient.get<CustomerAddress[]>(this.API);
  }

  AddAddress(newAddress: CustomerAddress): Observable<CustomerAddress> {
    return this.httpClient.post<CustomerAddress>(this.API, newAddress);
  }

  UpdateAddress(updatedAddress: CustomerAddress): Observable<CustomerAddress> {
    return this.httpClient.put<CustomerAddress>(this.API, updatedAddress);
  }

  DeleteAddress(addressId: number): Observable<HttpResponse<null>> {
    return this.httpClient.delete<HttpResponse<null>>(this.API + addressId);
  }
}
