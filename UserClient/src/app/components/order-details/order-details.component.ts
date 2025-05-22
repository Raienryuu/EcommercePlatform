import { Component, Input } from '@angular/core';
import { SampleProducts } from 'src/app/develSamples';
import { Product } from 'src/app/models/product';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from 'src/app/services/orderService/order.service';
import { Order } from 'src/app/models/order.model';
import { environment } from 'src/enviroment';
import { ProductService } from 'src/app/services/productService/product.service';
import { Subscription } from 'rxjs';
import { IMAGE_LOADER } from '@angular/common';
import { imageLoader } from 'src/app/images/imageLoader';

@Component({
  selector: 'app-order-details',
  standalone: false,
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.scss',
  providers: [{ provide: IMAGE_LOADER, useValue: imageLoader }],
})
export class OrderDetailsComponent {
  Math = Math;
  products: Product[] = SampleProducts;
  currencySymbol: string | undefined;
  isLoaded = false;
  orderNotFound = false;
  order: Order | undefined;
  orderStatus = '';
  paymentStatus: string | undefined;

  @Input()
  orderId = '';

  paymentFormVisible = false;
  clientSecret: string | undefined;
  cancellationRequested = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private orderService: OrderService,
    private productService: ProductService,
  ) {
    this.orderId = this.activatedRoute.snapshot.paramMap.get('id')!;
    if (environment.sampleData === true) {
      this.isLoaded = true;
      //return;
    }
    if (this.orderId == null) {
      this.orderNotFound = true;
      return;
    }
    this.LoadOrder(this.orderId).add(() => {
      if (environment.sampleData) {
        this.orderNotFound = false;
      }
      if (this.order == null) {
        throw new Error('Trying to get products without order');
      }

      this.LoadOrderProducts(this.order);

      this.currencySymbol = this.order.currencyISO; // map to symbol instead
    });
    this.LoadPaymentStatus(this.orderId);
  }

  LoadOrder(orderId: string): Subscription {
    return this.orderService.GetOrder(orderId).subscribe({
      next: (order) => {
        this.order = order;
        console.log(order);

        this.orderStatus = order.status;

        this.isLoaded = true;
      },
      error: () => {
        this.orderNotFound = true;
        this.isLoaded = true;
      },
    });
  }

  LoadOrderProducts(order: Order) {
    const productIds: string[] = order.products.map<string>(
      (original) => original.productId,
    );
    this.productService
      .GetProductsBatch(productIds)
      .subscribe((freshProducts) => {
        this.products = freshProducts;
        this.products.forEach((p) => {
          const orderProduct = this.order?.products.find(
            (x) => x.productId == p.id,
          );
          if (orderProduct == null) {
            throw new Error('Unable to find matching product');
          }
          p.price = orderProduct.price;
          p.quantity = orderProduct.quantity;
        });
      });
  }

  LoadPaymentStatus(orderId: string) {
    this.orderService.GetPaymentStatus(orderId).subscribe((paymentStatus) => {
      this.paymentStatus = paymentStatus;
    });
  }

  ShowPaymentForm() {
    this.orderService
      .CreatePaymentIntent(this.orderId)
      .subscribe((response) => {
        this.clientSecret = response.body!;
        this.paymentFormVisible = true;
      });
  }

  GetPreviewImageSource(productId: string): string {
    return 'p-' + productId + '-0';
  }

  CancelOrder() {
    this.orderService
      .CancelOrder(this.orderId)
      .subscribe(() => (this.cancellationRequested = true));
  }
}
