import { IMAGE_LOADER } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
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
export class CartComponent {
  products: Product[] = environment.sampleData ? SampleProducts : [];
  unavailableProducts: string[] = [];
  constructor(
    private userSettingsService: UserSettingsService,
    private cartService: CartService,
    private productService: ProductService,
    private router: Router,
    private orderService: OrderService,
  ) {
    this.cartService.cartUpdatedEvent.subscribe(() => {
      this.RecalculateTotalCost();
    });
    this.RecalculateTotalCost();
    this.GetCartContent();
  }

  cartUpdateDelayedEvent = new Subject();

  GetCartContent() {
    const productsList = this.cartService.GetCartProductsIds();
    this.productService.GetProductsBatch(productsList).subscribe((products) => {
      this.products = [];
      products.forEach((p) => {
        const localProduct = this.cartService.localCart.products.find(
          (fnd) => fnd.id === p.id.toString(),
        );
        p.quantity = this.GetProductAmount(p.quantity, localProduct!.amount);
        this.products.push(p);
        if (p.quantity === 0) {
          this.unavailableProducts.push(p.id);
        }
      });
      this.RecalculateTotalCost();
    });
  }

  private GetProductAmount(storage: number, needed: number) {
    if (storage >= needed) {
      return needed;
    } else {
      return storage;
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

  HandleQuantityChange(product: Product) {
    this.ValidateQuantity(product);
    this.SynchronizeCart(product);
  }

  AddQuantity(product: Product, quantity: number) {
    product.quantity += quantity;
    this.ValidateQuantity(product);
    this.SynchronizeCart(product);
  }

  SynchronizeCart(product: Product) {
    console.log('updating ', product);
    this.cartService.ChangeProductQuantity(product.id, product.quantity);
  }

  MAX_QUANTITY = 100;

  ValidateQuantity(product: Product): boolean {
    if (product.quantity < 1) {
      const productIndex = this.products.findIndex((p) => p.id === product.id);
      this.products.splice(productIndex, 1);
    }
    if (product.quantity > this.MAX_QUANTITY) {
      product.quantity = this.MAX_QUANTITY;
    }

    return true;
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
        // this.cartService.Clear();
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
