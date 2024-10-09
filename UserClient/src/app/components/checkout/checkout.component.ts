import {
  Component,
  EventEmitter,
  HostListener,
  Inject,
  Input,
  Output,
  ViewChild,
  inject,
  signal,
} from '@angular/core';
import { Product } from 'src/app/models/product';
import intlTelInput from 'intl-tel-input';
import { CountriesNoPhonesSorted } from '../register/RegisterRawData';
import { FormsModule, UntypedFormBuilder, Validators } from '@angular/forms';
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

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent {
  products: Product[] = null!;
  promoCodes: String[] = null!;
  currencySymbol: String = '€';
  total = { base: 155.88, tax: 15.88, delivery: 15, total: 170.88 };

  customerAddresses: CustomerAddress[] = [
    {
      Name: 'John',
      Lastname: 'California',
      Address: '787 Dunbar Road',
      Email: 'johnyboy@mail.com',
      PhonePrefix: '+1',
      PhoneNumber: '(213) 555-3890',
      City: 'San Jose, CA',
      ZIPCode: '95127',
      Country: 'USA',
    },
    {
      Name: 'John Senior',
      Lastname: 'California',
      Address: '788B Dunbar Road',
      Email: 'oljohny@mail.com',
      PhonePrefix: '+1',
      PhoneNumber: '(213) 555-3890',
      City: 'San Jose, CA',
      ZIPCode: '95127',
      Country: 'USA',
    },
  ];
  activeSelection: Number = this.customerAddresses.length - 1;

  constructor(
    private factoryService: StripeFactoryService,
    public dialog: MatDialog
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
  paymentIntent: any;

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

  SelectAddress(id: Number) {
    this.activeSelection = id;
    console.log(id);
  }

  dhlAddress!: DhlAddress;

  openDialog(): void {
    const dialogRef = this.dialog.open(LockerSelectorDialog);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.dhlAddress = result;
    });
  }
}

@Component({
  selector: 'locker-selctor',
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
export class LockerSelectorDialog {
  constructor(
    public dialogRef: MatDialogRef<LockerSelectorDialog>,
    @Inject(MAT_DIALOG_DATA) public data: DhlAddress
  ) {}

  onNoClick(): void {}

  @HostListener('window:message', ['$event'])
  relayMessage(event: any): DhlAddress {
    let parseRes: DhlAddress;
    try {
      parseRes = JSON.parse(event.data);
      if (parseRes.sap !== undefined) {
        this.dialogRef.close(parseRes);
      }
    } catch {}
    return null!;
  }
}


interface DhlAddress {
  sap: Number;
  name: String;
  zip: String;
  city: String;
  street: String;
  streetNo: String;
  houseNo: String;
}
