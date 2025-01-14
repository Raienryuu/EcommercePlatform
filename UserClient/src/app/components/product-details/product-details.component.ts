import { IMAGE_LOADER } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { imageLoader } from 'src/app/images/imageLoader';
import { ProductImagesMetadata } from 'src/app/images/ProductImagesMetadata';
import { Product } from 'src/app/models/product';
import { ImageService } from 'src/app/services/imagesService/images.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { environment } from 'src/enviroment';
@Component({
  selector: 'app-product-details',
  standalone: false,
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
  providers: [{
    provide: IMAGE_LOADER,
    useValue: imageLoader,
  }],
})
export class ProductDetailsComponent implements OnInit {
  /**
   *
   */
  constructor(private route: ActivatedRoute,
    private productService: ProductService,
    private imageService: ImageService
  ) { }

  product: Product = null!;
  imagesMetadata: ProductImagesMetadata = null!;
  noProductFound = false;
  isLoading = true;
  imagesRange: Array<number> = [];
  currentImageSrc: string = '';

  ngOnInit(): void {

    if (!environment.sampleData) {
      this.productService.GetProductById(this.id).subscribe(
        {
          next: (response) => {
            this.product = response;
            this.isLoading = false;
          },
          error: () => {
            this.isLoading = false;
            this.noProductFound = true;
          }
        });

      this.imageService.GetProductImagesMetadata(this.id)
        .subscribe(data => {
          this.imagesMetadata = data;
          this.imagesRange = Array.from(Array(this.imagesMetadata.numberOfImages));
        });
    }
    else {

      this.product = {
        name: "WOW Cataclysm TROLL MAGE EU - PvP Giantstalker Level 30",
        id: this.id,
        price: 88.32,
        quantity: 5,
        description: "to be implmented",
        categoryId: 1
      }
      this.isLoading = false;
      this.imagesMetadata = {
        numberOfImages: 9,
        productId: this.product.id,
      }
    }
    this.currentImageSrc = 'p-' + this.id + '-0';
    this.imagesRange = Array.from(Array(this.imagesMetadata.numberOfImages).keys());
  }

  currencySymbol = "€";
  @Input()
  id = parseInt(this.route.snapshot.paramMap.get("id")!) ?? 0;

  SelectPhoto(photoNumber: number) {
    console.info("Changing to " + photoNumber);
  }
}
