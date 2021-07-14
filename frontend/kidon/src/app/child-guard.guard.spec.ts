import { TestBed } from '@angular/core/testing';

import { ChildGuardGuard } from './child-guard.guard';

describe('ChildGuardGuard', () => {
  let guard: ChildGuardGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ChildGuardGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
