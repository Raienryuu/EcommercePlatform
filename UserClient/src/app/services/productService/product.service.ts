import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from 'src/app/models/product';
import { environment } from 'src/enviroment';
import { SearchFilters } from 'src/app/models/search-filters';
import * as currency from 'currency.js';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private httpClient: HttpClient) { }

  GetProductById(id: number): Observable<Product> {
    var url = environment.apiUrl + `v1/products/${id}`;
    return this.httpClient.get<Product>(url);
  }

  GetProductsPage(pageNum: number, pageSize: number,
    filters: SearchFilters): Observable<Product[]> {
    var url = environment.apiUrl + "v1/products/" + pageNum + "/" + pageSize;

    var url = ApplySearchFilters(url, filters);
    return this.httpClient
      .get<Product[]>(url);
  }

  GetNextPage(pageSize: number, filters: SearchFilters,
    edgeProduct: Product): Observable<Product[]> {
    var url = environment.apiUrl + `v1/products/nextPage/${pageSize}`;
    var url = ApplySearchFilters(url, filters);
    return this.httpClient.request<Product[]>('get', url, { body: { edgeProduct } });
  }

  GetPreviousPage(pageSize: number, filters: SearchFilters,
    edgeProduct: Product): Observable<Product[]> {
    var url = environment.apiUrl + `v1/products/previousPage/${pageSize}`;

    var url = ApplySearchFilters(url, filters);
    return this.httpClient.request<Product[]>('get', url, { body: { edgeProduct } });
  }
}

function ApplySearchFilters(url: string, filters: SearchFilters): string {
  if (filters === undefined)
    return url;

  url = url.concat("?")
  if (filters.Name !== "")
    url = url.concat("&Name=" + filters.Name);

  if (filters.MinPrice > currency(0))
    url = url.concat("&MinPrice=" + filters.MinPrice);

  if (filters.MaxPrice > currency(0))
    url = url.concat("&MaxPrice=" + filters.MaxPrice);

  if (filters.MinQuantity != 0)
    url = url.concat("&MinQuantity=" + filters.MinQuantity);

  if (filters.Order)
    url = url.concat("&Order=" + filters.Order);

  if (filters.Categories != 0)
    url = url.concat("&Categories=" + filters.Categories);
  return url;
}
