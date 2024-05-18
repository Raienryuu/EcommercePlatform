import { Component } from '@angular/core';
import { Product } from 'src/app/models/product';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent {
  products: Product[] = [
    { id: 2, categoryId: 3, name: "Super long product name that will take multiple lines of text", description: "Description for Product B", price: 19.99, quantity: 5 },
    { id: 3, categoryId: 1, name: "Super long product name that will take multiple lines of text under the product's photo that is yet to be changed", description: "Description for Product C", price: 14.99, quantity: 8 },
    { id: 4, categoryId: 2, name: "Short D", description: "Description for Product D", price: 24.99, quantity: 3 },
  ];

  constructor(private userSettingsService: UserSettingsService) { }

  ngOnInit() {
    this.RecalculateTotalCost();
  }

  currencySymbol: string = '€';
  promoCode: string = '';
  totalCost: { price: string, tax: string, total: string } = { price: '', tax: '', total: '' };

  LoadUserSettings() {
    this.userSettingsService
      .GetCurrencySymbol()
      .subscribe(symbol => this.currencySymbol = symbol);
  }

  GetProductTotal(product: Product): string {
    return (product.price * product.quantity).toFixed(2);
  }

  RecalculateTotalCost() {
    let priceSum: number = 0;
    let taxSum: number = 0;
    let totalSum: number = 0;
    this.products.forEach(p => {
      priceSum += p.price * p.quantity;
    })
    taxSum = priceSum * 0.23;
    priceSum -= taxSum;
    totalSum = priceSum + taxSum;

    this.totalCost = { price: priceSum.toFixed(2), tax: taxSum.toFixed(2), total: totalSum.toFixed(2) };
  }

  AddQuantity(product: Product, quantity: number) {
    product.quantity += quantity;
    if (product.quantity < 0)
      product.quantity = 0;
    this.RecalculateTotalCost();
  }

}
