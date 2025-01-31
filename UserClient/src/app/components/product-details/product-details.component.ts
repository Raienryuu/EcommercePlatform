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


  product: Product = null!;
  imagesMetadata: ProductImagesMetadata = {
    productId: 0,
    storedImages: [],
  }
  noProductFound = false;
  isLoading = true;
  currentImageSrc: string = '';

  constructor(private route: ActivatedRoute,
    private productService: ProductService,
    private imageService: ImageService
  ) { }

  ngOnInit() {
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
          this.currentImageSrc = this.imagesMetadata.storedImages[0];
          this.selectedImageNumber = parseInt(this.imagesMetadata.storedImages[0].split('-')[2]);
        });
    }
    else { // Sample data route
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
        productId: this.product.id,
        storedImages: ["p-12-0", "p-12-1", "p-12-2"],
      }
      this.currentImageSrc = this.imagesMetadata.storedImages[0];
      this.selectedImageNumber = parseInt(this.imagesMetadata.storedImages[0].split('-')[2]);
    }
  }

  currencySymbol = "€";
  @Input()
  id = parseInt(this.route.snapshot.paramMap.get("id")!) ?? 0;

  SelectPhoto(photoNumber: number) {
    console.info("Changing to " + photoNumber);
  }

  selectedImageNumber: number = 0;
  ChangeFocusedImage(imageName: string) {
    const imageNumber = this.GetNumberFromImageMetaName(imageName);
    console.info("Changing selected image to number: ", imageNumber);
    this.selectedImageNumber = imageNumber;
    this.ScrollImagesPreviews(imageNumber);
  }

  GetNumberFromImageMetaName(name: string): number {
    return parseInt(name.split('-')[2]);
  }

  private ScrollImagesPreviews(imageNumber: number) {
    const previewImageWidth = 120;
    const element = document.getElementById("preview");
    element?.scroll({ left: imageNumber * previewImageWidth - 120 });
  }

  public AddToCart() {
    console.info("Clicked Add to cart button.")
  }
}
