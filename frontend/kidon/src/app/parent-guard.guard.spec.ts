import { TestBed } from '@angular/core/testing';

import { ParentGuardGuard } from './parent-guard.guard';

describe('ParentGuardGuard', () => {
  let guard: ParentGuardGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ParentGuardGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
