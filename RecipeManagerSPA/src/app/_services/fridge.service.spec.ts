/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { FridgeService } from './fridge.service';

describe('Service: Fridge', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [FridgeService]
    });
  });

  it('should ...', inject([FridgeService], (service: FridgeService) => {
    expect(service).toBeTruthy();
  }));
});
