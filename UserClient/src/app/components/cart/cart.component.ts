import { IMAGE_LOADER } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, debounceTime } from 'rxjs';
import { SampleProducts } from 'src/app/develSamples';
import { imageLoader } from 'src/app/images/imageLoader';
import { CreateOrderRequest } from 'src/app/models/create-order-request';
import { OrderProduct } from 'src/app/models/order-product.model';
import { Product } from 'src/app/models/product';
import { CartService } from 'src/app/services/cartService/cart.service';
import { OrderService } from 'src/app/services/orderService/order.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
import { environment } from 'src/enviroment';

@Component({
  selector: 'app-cart',
  standalone: false,
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
  providers: [
    {
      provide: IMAGE_LOADER,
      useValue: imageLoader,
    },
  ],
})
export class CartComponent implements OnInit {
  products: Product[] = environment.sampleData ? SampleProducts : [];
  constructor(
    private userSettingsService: UserSettingsService,
    private cartService: CartService,
    private productService: ProductService,
    private router: Router,
    private orderService: OrderService,
  ) {}

  cartUpdateDelayedEvent = new Subject();

  ngOnInit() {
    this.RecalculateTotalCost();
    this.GetCartContent();
  }

  GetCartContent() {
    this.products = [];
    const productsList = this.cartService.GetCartProductsIds();
    this.productService.GetProductsBatch(productsList).subscribe((products) => {
      products.forEach((p) => {
        const localProduct = this.cartService.localCart.products.find(
          (fnd) => fnd.id === p.id.toString(),
        );
        p.quantity = this.GetProductAmount(p.quantity, localProduct!.amount);
        this.products.push(p);
      });
      this.RecalculateTotalCost();
    });
  }

  private GetProductAmount(storage: number, needed: number) {
    if (storage > needed) {
      return needed;
    } else {
      return 0;
    }
  }

  currencySymbol = '€';
  promoCode = '';
  totalCost = '0';

  LoadUserSettings() {
    this.userSettingsService
      .GetCurrencySymbol()
      .subscribe((symbol) => (this.currencySymbol = symbol));
  }

  GetProductTotal(product: Product): string {
    return (product.price * product.quantity).toFixed(2);
  }

  RecalculateTotalCost() {
    let sum = 0;
    this.products.forEach((p) => (sum += p.price * p.quantity));
    this.totalCost = sum.toFixed(2);
  }

  AddQuantity(product: Product, quantity: number) {
    product.quantity += quantity;
    this.ValidateQuantity(product);

    if (this.cartUpdateDelayedEvent.observed) {
      this.cartUpdateDelayedEvent.next(null);
      return;
    }

    this.cartUpdateDelayedEvent.pipe(debounceTime(800)).subscribe({
      next: () => {
        this.cartService
          .ChangeProductQuantity(product.id, product.quantity)
          .subscribe(() => this.RecalculateTotalCost());
      },
    });
    this.cartUpdateDelayedEvent.next(null);
  }

  MAX_QUANTITY = 100;
  ValidateQuantity(product: Product) {
    if (product.quantity < 1) {
      const productIndex = this.products.findIndex((p) => p.id === product.id);
      this.products.splice(productIndex);
    }
    if (product.quantity > this.MAX_QUANTITY)
      product.quantity = this.MAX_QUANTITY;
  }

  IsDecrementationInvalid(product: Product): boolean {
    if (product.quantity <= 1) return true;
    return false;
  }
  IsIncrementationInvalid(product: Product): boolean {
    if (product.quantity >= this.MAX_QUANTITY) return true;
    return false;
  }

  ToCheckout() {
    this.orderService.CreateNewOrder(this.CreateOrder()).subscribe({
      next: (order) => {
        this.router.navigate([`/checkout/${order.orderId}`]);
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  private CreateOrder(): CreateOrderRequest {
    const mappedProducts: OrderProduct[] = this.products.map<OrderProduct>(
      (p) => ({
        productId: p.id,
        quantity: p.quantity,
        price: p.price,
      }),
    );

    const newOrder: CreateOrderRequest = {
      notes: null,
      products: mappedProducts,
      currencyISO: this.userSettingsService.GetCurrencyISO(),
    };
    return newOrder;
  }

  GetPreviewImageSource(productId: string): string {
    return 'p-' + productId + '-0';
  }
}
