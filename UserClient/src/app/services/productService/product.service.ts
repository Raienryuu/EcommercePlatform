import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { delay, Observable, of } from 'rxjs';
import { Product } from 'src/app/models/product';
import { environment } from 'src/enviroment';
import { PaginationParams } from 'src/app/models/pagination-params';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private httpClient: HttpClient) { }

  GetProductById(id: number): Observable<Product> {
    const url = environment.apiUrl + `v1/products/${id}`;
    return this.httpClient.get<Product>(url);
  }

  GetProductsPage(filters: PaginationParams): Observable<Product[]> {
    if (environment.sampleData === true) {
      const products: Product[] = [
        {
          id: 1,
          categoryId: 2,
          name: 'Longer name Product A',
          description: 'Description for Product A',
          price: 9.99,
          quantity: 10,
        },
        {
          id: 2,
          categoryId: 3,
          name: 'Super long product name that will take multiple lines of text',
          description: 'Description for Product B',
          price: 19.99,
          quantity: 5,
        },
        {
          id: 3,
          categoryId: 1,
          name: "Super long product name that will take multiple lines of text under the product's photo that is yet to be changed",
          description: 'Description for Product C',
          price: 14.99,
          quantity: 8,
        },
        {
          id: 4,
          categoryId: 2,
          name: 'Short D',
          description: 'Description for Product D',
          price: 24.99,
          quantity: 3,
        },
        {
          id: 5,
          categoryId: 3,
          name: 'Product E',
          description: 'Description for Product E',
          price: 12.99,
          quantity: 12,
        },
        {
          id: 6,
          categoryId: 1,
          name: 'Product F',
          description: 'Description for Product F',
          price: 29.99,
          quantity: 6,
        },
        {
          id: 7,
          categoryId: 2,
          name: 'Product G',
          description: 'Description for Product G',
          price: 17.99,
          quantity: 9,
        },
        {
          id: 8,
          categoryId: 3,
          name: 'Product H',
          description: 'Description for Product H',
          price: 21.99,
          quantity: 4,
        },
        {
          id: 9,
          categoryId: 1,
          name: 'Product I',
          description: 'Description for Product I',
          price: 7.99,
          quantity: 15,
        },
        {
          id: 10,
          categoryId: 2,
          name: 'Product J',
          description: 'Description for Product J',
          price: 11.99,
          quantity: 7,
        },
      ];

      return of(products).pipe(delay(1000));
    }

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
