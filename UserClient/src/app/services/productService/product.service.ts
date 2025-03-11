import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from 'src/app/models/product';
import { environment } from 'src/enviroment';
import { PaginationParams } from 'src/app/models/pagination-params';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private httpClient: HttpClient) {}

  GetProductById(id: string): Observable<Product> {
    const url = environment.apiUrl + `v1/products/${id}`;
    return this.httpClient.get<Product>(url);
  }

  GetProductsPage(filters: PaginationParams): Observable<Product[]> {
    let url = environment.apiUrl + 'v1/products';
    url = ApplySearchFilters(url, filters);
    return this.httpClient.get<Product[]>(url);
  }

  GetNextPage(
    filters: PaginationParams,
    edgeProduct: Product,
  ): Observable<Product[]> {
    if (environment.sampleData === true) {
      return this.GetProductsPage(null!);
    }
    let url = environment.apiUrl + `v1/products/nextPage`;
    url = ApplySearchFilters(url, filters);
    return this.httpClient.post<Product[]>(url, edgeProduct);
  }

  GetPreviousPage(
    filters: PaginationParams,
    edgeProduct: Product,
  ): Observable<Product[]> {
    if (environment.sampleData === true) {
      return this.GetProductsPage(null!);
    }
    let url = environment.apiUrl + `v1/products/previousPage`;
    url = ApplySearchFilters(url, filters);
    return this.httpClient.post<Product[]>(url, edgeProduct);
  }

  GetProductsBatch(productIds: number[]): Observable<Product[]> {
    const url = environment.apiUrl + 'v1/products/batch';
    return this.httpClient.post<Product[]>(url, productIds);
  }
}

function ApplySearchFilters(url: string, filters: PaginationParams): string {
  if (filters === undefined) return url;

  url = url.concat('?');

  if (filters.PageNum) url = url.concat('PageNum=' + filters.PageNum + '&');

  if (filters.PageSize) url = url.concat('PageSize=' + filters.PageSize + '&');

  if (filters.Name !== null) url = url.concat('Name=' + filters.Name + '&');

  if (filters.MinPrice) url = url.concat('MinPrice=' + filters.MinPrice + '&');

  if (filters.MaxPrice) url = url.concat('MaxPrice=' + filters.MaxPrice + '&');

  if (filters.MinQuantity)
    url = url.concat('MinQuantity=' + filters.MinQuantity + '&');

  if (filters.Order) url = url.concat('Order=' + filters.Order + '&');

  if (filters.Categories)
    url = url.concat('Categories=' + filters.Categories + '&');
  return url;
}
