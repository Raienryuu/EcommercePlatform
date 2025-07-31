import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ShouldShowLoggedCapabilitiesPipe } from 'src/app/pipes/should-show-logged-capabilities.pipe';
import { CartService } from 'src/app/services/cartService/cart.service';
import { InternalCommunicationService } from 'src/app/services/internalCommunicationService/internal-communication.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.scss'],
    imports: [
        MatIconModule,
        ShouldShowLoggedCapabilitiesPipe,
        MatButtonModule,
        RouterModule,
    ]
})
export class NavbarComponent {
  /**
   *
   */
  constructor(
    private userSettingsService: UserSettingsService,
    private cartService: CartService,
    public activatedRoute: ActivatedRoute,
    private internalCommunicationService: InternalCommunicationService,
  ) {
    this.LoadUserName();
    if (this.cartService.localCart.products.length > 0) {
      this.LoadCartProductsCount();
    }
    this.internalCommunicationService.userLoggedInEvent.subscribe(() => {
      this.LoadUserName();
    });
    this.internalCommunicationService.newProductInCartEvent.subscribe(
      (newCartItemsCount) => (this.CartItemsCount = newCartItemsCount),
    );
  }

  CartItemsCount = 0;
  Name = '';
  ShowLoggedCapabilities = true;
  IsLoggedIn = false;

  ShouldLoggedCapabilitiesBeVisible(): boolean {
    const route = this.activatedRoute.children[0].snapshot.url[0].path;

    return !(route.includes('login') || route.includes('register'));
  }

  LoadUserName() {
    this.userSettingsService.GetUserName().subscribe((name) => {
      this.Name = name;
      this.IsLoggedIn = true;
    });
  }

  LoadCartProductsCount() {
    this.cartService
      .GetCartProductsCount()
      .subscribe((count) => (this.CartItemsCount = count));
  }
}
