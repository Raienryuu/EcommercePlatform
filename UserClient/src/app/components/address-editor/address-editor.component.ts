import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  viewChild,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CountrySelectComponent } from 'src/app/country-select/country-select.component';
import { CustomerAddress } from 'src/app/models/customer-address.model';
import { AddressService } from 'src/app/services/addressService/address.service';

@Component({
  selector: 'app-address-editor',
  standalone: false,
  templateUrl: './address-editor.component.html',
  styleUrl: './address-editor.component.scss',
})
export class AddressEditorComponent {
  @Input() address: CustomerAddress = {
    Id: -1,
    FullName: '',
    Email: '',
    PhoneNumber: '',
    Address: '',
    City: '',
    Country: 'United States',
    ZIPCode: '',
  };


  @Input() isNew = false;

  @Output()
  actionResponse = new EventEmitter<AddressEditorResponse | undefined>();

  /**
   *
   */
  constructor(private addressService: AddressService,
    private changeDetectorRef : ChangeDetectorRef
  ) {}

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

  printAddress() {
    console.log(this.address);
  }
  
  CloseEditor(): void {
    this.actionResponse.emit(undefined);
  }

  AddAddress(): void {
    if (!this.IsDataValid()) return;

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

    this.addressService.DeleteAddress(this.address.Id).subscribe({
      next: () =>
        this.actionResponse.emit({ Address: this.address, WasDeleted: true }),
      error: () => console.error('unable to delete'),
      complete: () => console.warn('finished'),
    });
  }

  countrySelector = viewChild<CountrySelectComponent>('country');

  IsDataValid(): boolean {
    this.addressForm.markAllAsTouched();
    if (this.addressForm.invalid) return false;
    this.address = {
      Id: this.addressForm.controls['id'].value!,
      Address: this.addressForm.controls['address'].value!,
      City: this.addressForm.controls['city'].value!,
      Country: this.addressForm.controls['country'].value!,
      Email: this.addressForm.controls['email'].value!,
      PhoneNumber: this.addressForm.controls['phoneNumber'].value!,
      FullName: this.addressForm.controls['fullname'].value!,
      ZIPCode: this.addressForm.controls['zipcode'].value!,
    };
    return true;
  }
}

export interface AddressEditorResponse {
  WasDeleted: boolean;
  Address: CustomerAddress | undefined;
}
