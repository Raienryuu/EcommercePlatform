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

  GetProductsPage(pageNum: number, pageSize: number,
    filters: SearchFilters | undefined)
    : Observable<Product[]> {
    var requestUrl = environment.apiUrl + "v1/products/" + pageNum + "/" + pageSize;

    if (filters !== undefined)
      var requestUrl = ApplySearchFilters(requestUrl, filters);
    console.info("using this url to acces getPage: " + requestUrl);
    return this.httpClient
      .get<Product[]>(requestUrl);
  }
}

function ApplySearchFilters(url: string, filters: SearchFilters): string {
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
    url = url.concat("Order=" + filters.Order);

  if (filters.Categories != 0)
    url = url.concat("&Categories=" + filters.Categories);
  return url;
}

