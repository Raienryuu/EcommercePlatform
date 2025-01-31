import { TestBed } from '@angular/core/testing';

import { ImageService } from './images.service';
import { provideHttpClient } from '@angular/common/http';

describe('ImagesService', () => {
  let service: ImageService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient()]
    });
    service = TestBed.inject(ImageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
