import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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
    public activatedRoute: ActivatedRoute,
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
  ShowLoggedCapabilities = true;

  ShouldLoggedCapabilitiesBeVisible(): boolean {
    const route = this.activatedRoute.children[0].snapshot.url[0].path;
    console.log(this.activatedRoute.children[0]);

    return !(route.includes('login') || route.includes('register'));
  }
}
