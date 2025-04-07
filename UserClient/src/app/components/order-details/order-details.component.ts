import { Component } from '@angular/core';
import { SampleProducts } from 'src/app/develSamples';
import { Product } from 'src/app/models/product';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-order-details',
  standalone: false,
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.scss',
})
export class OrderDetailsComponent {
  products: Product[] = SampleProducts;
  currencySymbol: string | undefined;
  /**
   *
   */
  constructor(
    private activatedRoute: ActivatedRoute,
    private userSettings: UserSettingsService,
  ) {
    userSettings.GetCurrencySymbol().subscribe((symbol) => {
      this.currencySymbol = symbol;
    });
  }
}
