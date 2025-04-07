import { Component, EventEmitter, Input, ViewChild } from '@angular/core';
import { Product } from 'src/app/models/product';
import { environment } from 'src/enviroment';
import { CustomerAddress } from 'src/app/models/customer-address.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import {
  AddressEditorComponent,
  AddressEditorResponse,
} from '../address-editor/address-editor.component';
import { MatRadioChange } from '@angular/material/radio';
import { SampleCustomerAddresses, SampleProducts } from 'src/app/develSamples';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from 'src/app/services/orderService/order.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { Order } from 'src/app/models/order.model';
import { map, retry } from 'rxjs';
import { LockerSelectorDialogComponent } from '../dhl-locker/dhl-locker.component';
import { DhlAddress } from 'src/app/models/dhl-address.model';
import { StripePaymentComponent } from '../stripe-payment/stripe-payment.component';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
  standalone: false,
})
export class CheckoutComponent {
  products: Product[] = [];
  promoCodes: string[] = [];
  currencySymbol = '€';
  total = '0';

  customerAddresses: CustomerAddress[] = SampleCustomerAddresses;
  activeSelection: number = this.customerAddresses.length - 1;

  orderLoaded = false;
  orderNotReadyYet = false;
  orderNotFound = false;

  @Input()
  id: string | undefined;

  constructor(
    public dialogDhl: MatDialog,
    public dialogAddressEditor: MatDialog,
    private route: ActivatedRoute,
    private orderService: OrderService,
    private productService: ProductService,
  ) {
    if (environment.sampleData) {
      this.products = SampleProducts;
      this.RecalculateTotalCost();
      this.orderLoaded = true;
    }

    this.id = this.route.snapshot.paramMap.get('orderId')!;
    if (this.id == undefined) {
      this.products = [];
      throw new Error('Order id is null');
    }

    this.orderService.GetOrder(this.id).subscribe({
      next: (order) => {
        const productIds = order.products.map<string>((p) => p.productId);
        this.productService.GetProductsBatch(productIds).subscribe({
          next: (freshProducts) => this.UpdateProducts(order, freshProducts),
        });
      },
    });
    this.GetClientSecret();
  }

  private GetClientSecret() {
    if (this.id == null) {
      throw new Error('Order Id cannot be null');
    }

    this.orderService
      .CreatePaymentIntent(this.id)
      .pipe(
        map((res) => {
          if (res.status == 202) {
            throw new Error('Client secret is not ready yet.');
          }
          return res;
        }),
      )
      .pipe(retry({ count: 2, delay: 800 }))
      .subscribe({
        next: (paymentIntentResponse) => {
          this.YOUR_CLIENT_SECRET = paymentIntentResponse.body!;
          this.orderLoaded = true;
        },
      });
  }

  private UpdateProducts(order: Order, products: Product[]) {
    this.products = products.map((p) => {
      const productInOrder = order.products.find((x) => x.productId === p.id);

      if (!productInOrder) {
        throw new ReferenceError(
          'Unable to find product Ids from order in products batch',
        );
      }

      p.quantity = productInOrder.quantity;

      return p;
    });
    this.RecalculateTotalCost();
  }

  RecalculateTotalCost() {
    let sum = 0;
    this.products.forEach((p) => {
      sum += p.quantity * p.price;
    });
    this.total = sum.toFixed(2);
  }

  YOUR_CLIENT_SECRET: string | undefined;
  @ViewChild(StripePaymentComponent)
  stripeComponent!: StripePaymentComponent;

  OnClickOrderButtonHandler() {
    console.clear();
    this.stripeComponent.MakeStripePayment();
  }

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

    // delete address case
    if ($event.WasDeleted) {
      this.customerAddresses = this.customerAddresses.filter(
        (x) => x.Id !== $event.Address!.Id,
      );
      return;
    }
    let doesExists = false;

    // update address case
    this.customerAddresses.forEach((a, index) => {
      if (a.Id === $event!.Address!.Id) {
        this.customerAddresses[index] = $event!.Address!;
        doesExists = true;
      }
    });

    // add address case
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

//#endregion
