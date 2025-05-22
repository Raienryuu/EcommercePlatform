import { TestBed } from '@angular/core/testing';

import { InternalCommunicationService } from './internal-communication.service';

describe('InternalCommunicationServiceService', () => {
  let service: InternalCommunicationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(InternalCommunicationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
