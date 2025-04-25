import { Component } from '@angular/core';
import { CartService } from 'src/app/services/cartService/cart.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  /**
   *
   */
  constructor(
    private userSettingsService: UserSettingsService,
    private cartService: CartService,
  ) {
    this.userSettingsService.GetUserName().subscribe((name) => {
      this.Name = name;
    });
    this.cartService
      .GetCartProductsCount()
      .subscribe((count) => (this.CartItemsCount = count));
  }
  CartItemsCount = 0;
  Name = '';
}
