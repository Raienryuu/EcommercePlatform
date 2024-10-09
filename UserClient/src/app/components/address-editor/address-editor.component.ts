import { Component, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { CustomerAddressV2 } from 'src/app/models/customer-address.model';

@Component({
  selector: 'app-address-editor',
  standalone: false,
  templateUrl: './address-editor.component.html',
  styleUrl: './address-editor.component.scss',

})
export class AddressEditorComponent {
  address: CustomerAddressV2 = {
    Address: '',
    City: '',
    Country: '',
    Email: '',
    FullName: '',
    PhoneNumber: '',
    ZIPCode: '',
  };

  printAddress() {
    console.log(this.address);
  }
}
