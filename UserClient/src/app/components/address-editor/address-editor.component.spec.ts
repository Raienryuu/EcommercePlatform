import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddressEditorComponent } from './address-editor.component';

describe('AddressEditorComponent', () => {
  let component: AddressEditorComponent;
  let fixture: ComponentFixture<AddressEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddressEditorComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddressEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
