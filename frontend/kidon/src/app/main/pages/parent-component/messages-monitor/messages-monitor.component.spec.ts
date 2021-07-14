import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessagesMonitorComponent } from './messages-monitor.component';

describe('MessagesMonitorComponent', () => {
  let component: MessagesMonitorComponent;
  let fixture: ComponentFixture<MessagesMonitorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MessagesMonitorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MessagesMonitorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
