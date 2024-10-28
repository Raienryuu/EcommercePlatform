import { TestBed } from '@angular/core/testing';

import { AddressService } from './address.service';
import { provideHttpClient } from '@angular/common/http';

describe('AddressService', () => {
  let service: AddressService;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [provideHttpClient()]});
    service = TestBed.inject(AddressService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
