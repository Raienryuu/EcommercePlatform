import {
  Component,
  EventEmitter,
  Input,
  Output,
  viewChild,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { NgxMatInputTelComponent } from 'ngx-mat-input-tel';
import { CountrySelectComponent } from 'src/app/components/country-select/country-select.component';
import { CustomerAddress } from 'src/app/models/customer-address.model';
import { AddressService } from 'src/app/services/addressService/address.service';

@Component({
  selector: 'app-address-editor',
  standalone: true,
  templateUrl: './address-editor.component.html',
  styleUrl: './address-editor.component.scss',
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    CountrySelectComponent,
    NgxMatInputTelComponent,
    ReactiveFormsModule,
    MatInputModule,
  ],
})
export class AddressEditorComponent {
  @Input() address: CustomerAddress = {
    id: -1,
    fullName: '',
    email: '',
    phoneNumber: '',
    address: '',
    city: '',
    country: 'United States',
    zipCode: '',
  };

  @Input() isNew = false;

  @Output()
  actionResponse = new EventEmitter<AddressEditorResponse | undefined>();
  addressForm = new FormGroup({
    id: new FormControl(0),
    fullname: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    phoneNumber: new FormControl(
      '',
      Validators.compose([
        Validators.required,
        Validators.pattern('^[0-9 ()+-]*$'),
      ]),
    ),
    address: new FormControl('', Validators.required),
    city: new FormControl('', Validators.required),
    country: new FormControl('', Validators.required),
    zipcode: new FormControl('', Validators.required),
  });
  countrySelector = viewChild<CountrySelectComponent>('country');

  /**
   *
   */
  constructor(private addressService: AddressService) {}

  CloseEditor(): void {
    this.actionResponse.emit(undefined);
  }

  AddAddress(): void {
    if (!this.IsDataValid()) throw new Error('Address is invalid');

    this.addressService
      .AddAddress(this.address)
      .subscribe((x) =>
        this.actionResponse.emit({ Address: x, WasDeleted: false }),
      );
  }

  UpdateAddress(): void {
    if (!this.IsDataValid()) return;

    this.addressService
      .UpdateAddress(this.address)
      .subscribe((x) =>
        this.actionResponse.emit({ Address: x, WasDeleted: false }),
      );
  }

  DeleteAddress(): void {
    if (!this.IsDataValid()) return;

    this.addressService.DeleteAddress(this.address.id).subscribe({
      next: () =>
        this.actionResponse.emit({ Address: this.address, WasDeleted: true }),
      error: () => console.error('unable to delete'),
      complete: () => {
        return;
      },
    });
  }

  IsDataValid(): boolean {
    this.addressForm.markAllAsTouched();
    this.countrySelector()?.countryControl.markAsTouched();

    if (this.addressForm.errors !== null) return false;

    this.address = {
      id: this.addressForm.controls['id'].value!,
      address: this.addressForm.controls['address'].value!,
      city: this.addressForm.controls['city'].value!,
      country: this.addressForm.controls['country'].value!,
      email: this.addressForm.controls['email'].value!,
      phoneNumber: this.addressForm.controls['phoneNumber'].value!,
      fullName: this.addressForm.controls['fullname'].value!,
      zipCode: this.addressForm.controls['zipcode'].value!,
    };
    return true;
  }
}

export interface AddressEditorResponse {
  WasDeleted: boolean;
  Address: CustomerAddress | undefined;
}
