import { Component, Input, ViewChild } from '@angular/core';
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
import { Subscription, map, retry, tap } from 'rxjs';
import { LockerSelectorDialogComponent } from '../dhl-locker/dhl-locker.component';
import { DhlAddress } from 'src/app/models/dhl-address.model';
import { StripePaymentComponent } from '../stripe-payment/stripe-payment.component';
import { IMAGE_LOADER } from '@angular/common';
import { imageLoader } from 'src/app/images/imageLoader';
import { DeliveryService } from 'src/app/services/deliveryService/delivery.service';
import { DeliveryMethod } from 'src/app/models/delivery-method.model';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
  standalone: false,
  providers: [{ provide: IMAGE_LOADER, useValue: imageLoader }],
})
export class CheckoutComponent {
  products: Product[] = [];
  promoCodes: string[] = [];
  currencySymbol = '€';
  total = '0';

  customerAddresses: CustomerAddress[] = SampleCustomerAddresses;
  activeAddressSelection: number = this.customerAddresses.length - 1;

  orderLoaded = false;
  orderNotReadyYet = false;
  orderNotFound = false;

  @Input()
  id: string | undefined;

  deliveryMethods: DeliveryMethod[] = [];
  showPaymentForm = false;

  constructor(
    public dialogDhl: MatDialog,
    public dialogAddressEditor: MatDialog,
    private route: ActivatedRoute,
    private orderService: OrderService,
    private productService: ProductService,
    private deliveryService: DeliveryService,
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
    this.deliveryService
      .GetAvailableDeliveries()
      .pipe(tap((x) => console.log('got these deliveries: ', x)))
      .subscribe(
        (deliveries) =>
          (this.deliveryMethods = deliveries.sort((a, b) => a.price - b.price)),
      );

    this.orderService.GetOrder(this.id).subscribe({
      next: (order) => {
        const productIds = order.products.map<string>((p) => p.productId);
        this.productService.GetProductsBatch(productIds).subscribe({
          next: (freshProducts) => this.UpdateProducts(order, freshProducts),
        });
      },
    });
  }

  private GetClientSecret(): Subscription {
    if (this.id == null) {
      throw new Error('Order Id cannot be null');
    }

    return this.orderService
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
      p.price = productInOrder.price;

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
    this.SetDeliveryMethod().add(
      this.GetClientSecret().add(() => (this.showPaymentForm = true)),
    );
  }

  private SetDeliveryMethod() {
    if (this.id == null) {
      throw new Error('Id is invalid');
    }
    console.log(this.deliveryOptionValue);

    const delivery = this.deliveryMethods.find(
      (x: DeliveryMethod) => x.deliveryId === this.deliveryOptionValue,
    );
    if (delivery == null) {
      throw new Error('Delivery method is invalid');
    }
    delivery.customerInformation =
      this.customerAddresses[this.activeAddressSelection];
    return this.orderService
      .SetOrderDeliveryMethod(this.id, delivery)
      .subscribe();
  }

  SelectAddress(id: number) {
    this.activeAddressSelection = id;
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
    console.log($event);

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

  GetPreviewImageSource(productId: string): string {
    return 'p-' + productId + '-0';
  }
}
