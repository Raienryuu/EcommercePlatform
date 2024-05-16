import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCategory } from 'src/app/models/product-category';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root'
})
export class ProductCategoryService {

  constructor(private httpClient: HttpClient) { }

  GetCategoryChildren(categoryId: string): Observable<ProductCategory[]> {
    let url = environment.apiUrl + "v1/productscategories/children/" + categoryId;
    return this.httpClient.get<ProductCategory[]>(url);
  }
}
