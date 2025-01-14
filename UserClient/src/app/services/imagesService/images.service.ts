import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductImagesMetadata } from 'src/app/images/ProductImagesMetadata';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  constructor(private httpClient: HttpClient) { }

  GetProductImagesMetadata(id: number): Observable<ProductImagesMetadata> {
    return this.httpClient.get<ProductImagesMetadata>(environment.apiUrl + `v1/image/meta?${id}`);
  }
}
