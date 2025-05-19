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
import { Subscription, map, retry } from 'rxjs';
import { LockerSelectorDialogComponent } from '../dhl-locker/dhl-locker.component';
import { DhlAddress } from 'src/app/models/dhl-address.model';
import { StripePaymentComponent } from '../stripe-payment/stripe-payment.component';
import { IMAGE_LOADER } from '@angular/common';
import { imageLoader } from 'src/app/images/imageLoader';
import { DeliveryService } from 'src/app/services/deliveryService/delivery.service';
import { DeliveryMethod } from 'src/app/models/delivery-method.model';
import { OrderDelivery } from 'src/app/models/order-delivery.model';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
import { AddressService } from 'src/app/services/addressService/address.service';

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
    private userSettingsService: UserSettingsService,
    private addressService: AddressService,
  ) {
    this.userSettingsService
      .GetCurrencySymbol()
      .subscribe((symbol) => (this.currencySymbol = symbol));

    this.addressService
      .GetAddresses()
      .subscribe((addresses) => (this.customerAddresses = addresses));

    if (environment.sampleData) {
      this.deliveryMethods = [
        {
          name: 'DHL Parcel Locker',
          deliveryId: '5',
          price: 5,
          deliveryType: 'DeliveryPoint',
          paymentType: 'Online',
          handlerName: 'dhl',
        },
      ];
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

    const delivery = this.deliveryMethods.find(
      (x: DeliveryMethod) => x.deliveryId === this.deliveryOptionValue,
    );

    if (delivery == null) {
      throw new Error('Delivery method is invalid');
    }

    const deliveryDetails = this.ToOrderDelivery(
      delivery,
      this.customerAddresses[this.activeAddressSelection],
    );

    return this.orderService
      .SetOrderDeliveryMethod(this.id, deliveryDetails)
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
        (x) => x.id !== $event.Address!.id,
      );
      dialog.close();
      return;
    }
    let doesExists = false;

    // update address case
    this.customerAddresses.forEach((a, index) => {
      if (a.id === $event!.Address!.id) {
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
    form.controls['id'].setValue(newValue.id);
    form.controls['address'].setValue(newValue.address);
    form.controls['city'].setValue(newValue.city);
    form.controls['country'].setValue(newValue.country);
    form.controls['email'].setValue(newValue.email);
    form.controls['phoneNumber'].setValue(newValue.phoneNumber);
    form.controls['fullname'].setValue(newValue.fullName);
    form.controls['zipcode'].setValue(newValue.zipCode);

    dialog.componentInstance.countrySelector()?.writeValue(newValue.country);
  }

  dhlAddress!: DhlAddress;
  deliveryOptionValue: string | undefined;

  ChangeDeliveryValue($event: MatRadioChange) {
    this.deliveryOptionValue = $event.value;
  }

  openDhlDialog(): void {
    // this.deliveryOptionValue = this.deliveryMethods.find(
    //   (x) => x.name == 'DHL Parcel Locker',
    // )?.deliveryId;
    this.deliveryOptionValue = this.deliveryMethods[0].deliveryId;

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

  private ToOrderDelivery(
    delivery: DeliveryMethod,
    customerInformation: CustomerAddress,
  ): OrderDelivery {
    return {
      deliveryId: delivery.deliveryId,
      externalDeliveryId: null,
      customerInformation: customerInformation,
      deliveryType: delivery.deliveryType,
      handlerName: delivery.handlerName,
      name: delivery.name,
      paymentType: delivery.paymentType,
      price: delivery.price,
    };
  }
}
