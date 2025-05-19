import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LockerSelectorDialogComponent } from './dhl-locker.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

describe('DhlLockerComponent', () => {
  let component: LockerSelectorDialogComponent;
  let fixture: ComponentFixture<LockerSelectorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        MatFormFieldModule,
        MatInputModule,
        FormsModule,
        MatButtonModule,
        LockerSelectorDialogComponent,
      ],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: MAT_DIALOG_DATA },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LockerSelectorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
