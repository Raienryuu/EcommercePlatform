import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductImagesMetadata } from 'src/app/images/ProductImagesMetadata';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  constructor(private httpClient: HttpClient) {}

  GetProductImagesMetadata(id: string): Observable<ProductImagesMetadata> {
    return this.httpClient.get<ProductImagesMetadata>(
      environment.tempImagesUrl + `v1/imageMetadata?productId=${id}`,
    );
  }
}
