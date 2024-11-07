import {
  Component,
  HostListener,
  Inject,
  ViewChild,
} from '@angular/core';
import { Product } from 'src/app/models/product';
import { FormsModule } from '@angular/forms';
import {
  StripeFactoryService,
  StripePaymentElementComponent,
} from 'ngx-stripe';
import {
  StripeElementsOptions,
  StripePaymentElementOptions,
} from '@stripe/stripe-js';
import { environment } from 'src/enviroment';
import { CustomerAddress } from 'src/app/models/customer-address.model';
import { MatButtonModule } from '@angular/material/button';
import {
  MatDialog,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
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

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent {
  products: Product[] = null!;
  promoCodes: string[] = null!;
  currencySymbol = '€';
  total = { base: 155.88, tax: 15.88, delivery: 15, total: 170.88 };

  customerAddresses: CustomerAddress[] = [
    {
      Id: 2,
      FullName: 'John California',
      Address: '787 Dunbar Road',
      Email: 'johnyboy@mail.com',
      PhoneNumber: '+1 (213) 555-3890',
      City: 'San Jose, CA',
      ZIPCode: '95127',
      Country: 'United States',
    },
    {
      Id: 1,
      FullName: 'John Senior California',
      Address: '788B Dunbar Road',
      Email: 'oljohny@mail.com',
      PhoneNumber: '+1 (213) 555-3890',
      City: 'San Jose, CA',
      ZIPCode: '95127',
      Country: 'United States',
    },
  ];
  activeSelection: number = this.customerAddresses.length - 1;

  constructor(
    private factoryService: StripeFactoryService,
    public dialogDhl: MatDialog,
    public dialogAddressEditor: MatDialog,
  ) {
    this.products = [
      {
        id: 5,
        categoryId: 3,
        name: 'Product E',
        description: 'Description for Product E',
        price: 12.99,
        quantity: 12,
      },
    ];
  }

  stripe = this.factoryService.create(environment.stripeApiKey);
  YOUR_CLIENT_SECRET: string | null =
    'pi_3POFD5C7yfdpfbDs1K4y1MF5_secret_mbpshCsI0kRJtroB8J1zNQXNm';
  paymentIntent: unknown;

  @ViewChild(StripePaymentElementComponent)
  paymentElement!: StripePaymentElementComponent;

  elementsOptions: StripeElementsOptions = {
    locale: 'en',
    clientSecret: this.YOUR_CLIENT_SECRET!,
    appearance: {
      theme: 'stripe',
      variables: {
        colorBackground: '#F0F2FF',
        colorText: '#000000de',
        fontLineHeight: '50px',

        // fontSizeBase: '20px'
      },
      labels: 'floating',
      rules: {
        '.Input': {
          lineHeight: '1.5rem',
        },
      },
    },
  };
  paymentElementOptions: StripePaymentElementOptions = {
    layout: {
      type: 'tabs',
      defaultCollapsed: false,
    },
  };

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
  
  ChangeDeliveryValue($event: MatRadioChange){
    this.deliveryOptionValue = $event.value;
  }

  openDhlDialog(): void {
    this.deliveryOptionValue = 'dhl';
    
    const dialogRef = this.dialogDhl.open(LockerSelectorDialogComponent, {
      panelClass: 'dialogPanel',
    });
    // Does this even works??
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
  imports: [
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
  ],
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
        console.warn(event.data);
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