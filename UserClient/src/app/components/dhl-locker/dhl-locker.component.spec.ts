import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DhlLockerComponent } from './dhl-locker.component';

describe('DhlLockerComponent', () => {
  let component: DhlLockerComponent;
  let fixture: ComponentFixture<DhlLockerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DhlLockerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DhlLockerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
