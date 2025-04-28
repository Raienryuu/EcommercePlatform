import { TestBed } from '@angular/core/testing';

import { InternalCommunicationServiceService } from './internal-communication-service.service';

describe('InternalCommunicationServiceService', () => {
  let service: InternalCommunicationServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(InternalCommunicationServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
