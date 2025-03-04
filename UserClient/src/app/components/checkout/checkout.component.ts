import { Component, HostListener, Inject, ViewChild } from '@angular/core';
import { Product } from 'src/app/models/product';
import { FormsModule } from '@angular/forms';
import {
  StripeFactoryService,
  StripePaymentElementComponent,
} from 'ngx-stripe';
import { environment } from 'src/enviroment';
import { CustomerAddress } from 'src/app/models/customer-address.model';
import { MatButtonModule } from '@angular/material/button';
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
  AddressEditorComponent,
  AddressEditorResponse,
} from '../address-editor/address-editor.component';
import { MatRadioChange } from '@angular/material/radio';
import { SampleCustomerAddresses, SampleProducts } from 'src/app/develSamples';
import { StripeConfig } from 'src/app/stripe-settings';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
  standalone: false,
})
export class CheckoutComponent {
  products: Product[] = null!;
  promoCodes: string[] = null!;
  currencySymbol = '€';
  total = { base: 155.88, tax: 15.88, delivery: 15, total: 170.88 };

  customerAddresses: CustomerAddress[] = SampleCustomerAddresses;
  activeSelection: number = this.customerAddresses.length - 1;

  constructor(
    private factoryService: StripeFactoryService,
    public dialogDhl: MatDialog,
    public dialogAddressEditor: MatDialog,
  ) {
    this.products = SampleProducts;
  }

  stripe = this.factoryService.create(environment.stripeApiKey);
  stripeConfig = new StripeConfig();
  YOUR_CLIENT_SECRET = this.stripeConfig.YOUR_CLIENT_SECRET;
  paymentIntent: unknown;

  @ViewChild(StripePaymentElementComponent)
  paymentElement!: StripePaymentElementComponent;

  elementsOptions = this.stripeConfig.stripeElementsOptions;
  paymentElementOptions = this.stripeConfig.paymentElementOptions;
  SelectAddress(id: number) {
    this.activeSelection = id;
  }

  OpenAddressEditor(addNewAddress: boolean, addressesIndex: number) {
    const dialog = this.dialogAddressEditor.open(AddressEditorComponent, {
      panelClass: 'dialogPanel',
    });

    if (addNewAddress) dialog.componentRef!.instance.isNew = addNewAddress;
    else
      this.AssignAddressFormValues(
        dialog,
        this.customerAddresses[addressesIndex],
      );
    dialog.componentRef!.instance.actionResponse.subscribe((event) =>
      this.HandleAddressEvent(dialog, event),
    );
  }

  private HandleAddressEvent(
    dialog: MatDialogRef<AddressEditorComponent, unknown>,
    $event: AddressEditorResponse | undefined,
  ) {
    // just close dialog case
    if ($event === undefined) {
      dialog.close();
      return;
    }
    // delete case
    if ($event.WasDeleted) {
      this.customerAddresses = this.customerAddresses.filter(
        (x) => x.Id !== $event.Address!.Id,
      );
      return;
    }
    let doesExists = false;
    // update case
    this.customerAddresses.forEach((a, index) => {
      if (a.Id === $event!.Address!.Id) {
        this.customerAddresses[index] = $event!.Address!;
        doesExists = true;
      }
    });
    // add case
    if (!doesExists) this.customerAddresses.push($event!.Address!);
    dialog.close();
  }

  private AssignAddressFormValues(
    dialog: MatDialogRef<AddressEditorComponent, unknown>,
    newValue: CustomerAddress,
  ) {
    const form = dialog.componentInstance.addressForm;
    form.controls['id'].setValue(newValue.Id);
    form.controls['address'].setValue(newValue.Address);
    form.controls['city'].setValue(newValue.City);
    form.controls['country'].setValue(newValue.Country);
    form.controls['email'].setValue(newValue.Email);
    form.controls['phoneNumber'].setValue(newValue.PhoneNumber);
    form.controls['fullname'].setValue(newValue.FullName);
    form.controls['zipcode'].setValue(newValue.ZIPCode);

    dialog.componentInstance.countrySelector()?.writeValue(newValue.Country);
  }

  dhlAddress!: DhlAddress;
  deliveryOptionValue: string | undefined;

  ChangeDeliveryValue($event: MatRadioChange) {
    this.deliveryOptionValue = $event.value;
  }

  openDhlDialog(): void {
    this.deliveryOptionValue = 'dhl';

    const dialogRef = this.dialogDhl.open(LockerSelectorDialogComponent, {
      panelClass: 'dialogPanel',
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.dhlAddress = result;
    });
  }
}

//#region locker selector

@Component({
  selector: 'app-locker-selector',
  templateUrl: 'map.html',
  styleUrls: ['./checkout.component.scss'],
  standalone: true,
  imports: [MatFormFieldModule, MatInputModule, FormsModule, MatButtonModule],
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

interface DhlAddress {
  id: number;
  sap: number;
  name: string;
  zip: string;
  city: string;
  street: string;
  streetNo: string;
  houseNo: string;
}

//#endregion
