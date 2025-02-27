import { Component, OnInit } from '@angular/core';
import { Subject, debounceTime } from 'rxjs';
import { SampleProducts } from 'src/app/develSamples';
import { Product } from 'src/app/models/product';
import { CartService } from 'src/app/services/cartService/cart.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
import { environment } from 'src/enviroment';

@Component({
  selector: 'app-cart',
  standalone: false,
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class CartComponent implements OnInit {
  products: Product[] = environment.sampleData ? SampleProducts : [];
  constructor(
    private userSettingsService: UserSettingsService,
    private cartService: CartService,
    private productService: ProductService,
  ) {}

  cartUpdateDelayedEvent = new Subject();

  ngOnInit() {
    this.GetCartContent();
    this.RecalculateTotalCost();
  }
  GetCartContent() {
    const productsList = this.cartService.GetCartProductsIds();
    this.productService.GetProductsBatch(productsList).subscribe((products) => {
      products.forEach((p) => {
        const localProduct = this.cartService.localCart.Products.find(
          (fnd) => fnd.id === p.id.toString(),
        );
        p.quantity = localProduct!.amount;
        this.products.push(p);
      });
    });
  }

  currencySymbol = '€';
  promoCode = '';
  totalCost: { price: string; tax: string; total: string } = {
    price: '',
    tax: '',
    total: '',
  };

  LoadUserSettings() {
    this.userSettingsService
      .GetCurrencySymbol()
      .subscribe((symbol) => (this.currencySymbol = symbol));
  }

  GetProductTotal(product: Product): string {
    return (product.price * product.quantity).toFixed(2);
  }

  RecalculateTotalCost() {
    // TODO: ask API for calculation instead
    let priceSum = 0;
    let taxSum = 0;
    let totalSum = 0;
    this.products.forEach((p) => {
      priceSum += p.price * p.quantity;
    });
    taxSum = priceSum * 0.23;
    priceSum -= taxSum;
    totalSum = priceSum + taxSum;

    this.totalCost = {
      price: priceSum.toFixed(2),
      tax: taxSum.toFixed(2),
      total: totalSum.toFixed(2),
    };
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
          .subscribe((_) => this.RecalculateTotalCost());
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
    if (product.quantity === 1) return true;
    return false;
  }
  IsIncrementationInvalid(product: Product): boolean {
    if (product.quantity === this.MAX_QUANTITY) return true;
    return false;
  }
}
