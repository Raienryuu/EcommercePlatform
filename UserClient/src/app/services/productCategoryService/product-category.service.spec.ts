import { TestBed } from '@angular/core/testing';

import { ProductCategoryService } from './product-category.service';
import { provideHttpClient } from '@angular/common/http';

describe('ProductCategoryService', () => {
  let service: ProductCategoryService;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [provideHttpClient()]});
    service = TestBed.inject(ProductCategoryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
