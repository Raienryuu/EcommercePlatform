import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/productService/product.service';
import { environment } from 'src/enviroment';

@Component({
  selector: 'app-product-details',
  standalone: false,
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  /**
   *
   */
  constructor(private route: ActivatedRoute,
    private productService: ProductService,
  ) { }

  ngOnInit(): void {
    if (!environment.sampleData)
      this.productService.GetProductById(this.id).subscribe(
        {
          next: (response) =>
            this.product = response
        })
  }

  currencySymbol = "€";
  @Input()
  id = parseInt(this.route.snapshot.paramMap.get("id")!) ?? 0;
  product: Product = {
    name: "WOW Cataclysm TROLL MAGE EU - PvP Giantstalker Level 30",
    id: this.id,
    price: 88.32,
    quantity: 5,
    description: "to be implmented",
    categoryId: 1
  }
}
