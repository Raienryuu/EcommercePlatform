//#region locker selector

import { Component, HostListener, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DhlAddress } from 'src/app/models/dhl-address.model';

@Component({
    selector: 'app-dhl-locker',
    template: '<iframe class="map" src="https://parcelshop.dhl.pl/mapa"></iframe>',
    styleUrls: ['./dhl-locker.component.scss'],
    imports: [MatFormFieldModule, MatInputModule, FormsModule, MatButtonModule]
})
export class LockerSelectorDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<LockerSelectorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DhlAddress,
  ) {}

  onNoClick(): void {
    return;
  }

  @HostListener('window:message', ['$event'])
  relayMessage(event: MessageEvent): DhlAddress {
    try {
      const parseRes = JSON.parse(event.data);
      if (parseRes.sap !== undefined) {
        this.dialogRef.close(parseRes);
      }
    } catch {
      return null!;
    }
    return null!;
  }
}
