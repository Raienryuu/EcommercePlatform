import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomerAddress } from 'src/app/models/customer-address.model';

@Component({
  selector: 'app-address-editor',
  standalone: false,
  templateUrl: './address-editor.component.html',
  styleUrl: './address-editor.component.scss',

})
export class AddressEditorComponent {
  address: CustomerAddress = {
    FullName: '',
    Email: '',
    PhoneNumber: '',
    Address: '',
    City: '',
    Country: '',
    ZIPCode: '',
  };

  addressForm = new FormGroup({
    name: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    phoneNumber: new FormControl('', Validators.compose([
      Validators.required,
      Validators.pattern('^[0-9 ()+-]*$'),
    ])),
    address: new FormControl('', Validators.required),
    city: new FormControl('', Validators.required),
    country: new FormControl('', Validators.required),
    zipcode: new FormControl('', Validators.required),
  })

  printAddress() {
    console.log(this.address);
    console.log(this.addressForm.controls['country']);
    
  }
}
