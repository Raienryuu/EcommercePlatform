import { IMAGE_LOADER } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SampleImageMetadata, SampleProduct } from 'src/app/develSamples';
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
  providers: [
    {
      provide: IMAGE_LOADER,
      useValue: imageLoader,
    },
  ],
})
export class ProductDetailsComponent implements OnInit {
  /**
   *
   */

  product: Product = null!;
  imagesMetadata: ProductImagesMetadata = {
    productId: '',
    storedImages: [],
  };
  noProductFound = false;
  isLoading = true;
  currentImageSrc = 'invalid';

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private imageService: ImageService,
  ) {}

  ngOnInit() {
    if (!environment.sampleData) {
      this.productService.GetProductById(this.id).subscribe({
        next: (response) => {
          this.product = response;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.noProductFound = true;
        },
      });

      this.imageService.GetProductImagesMetadata(this.id).subscribe((data) => {
        this.imagesMetadata = data;
        this.currentImageSrc = this.imagesMetadata.storedImages[0];
        this.selectedImageNumber = parseInt(
          this.imagesMetadata.storedImages[0].split('-')[2],
        );
      });
    } else {
      // Sample data route
      this.product = SampleProduct;
      this.product.id = this.id;
      this.isLoading = false;
      this.imagesMetadata = SampleImageMetadata;
      this.imagesMetadata.productId = this.id;
      this.currentImageSrc = this.imagesMetadata.storedImages[0];
      this.selectedImageNumber = parseInt(
        this.imagesMetadata.storedImages[0].split('-')[2],
      );
    }
  }

  currencySymbol = '€';
  @Input()
  id = this.route.snapshot.paramMap.get('id') ?? '';

  selectedImageNumber = 0;
  ChangeFocusedImage(imageName: string) {
    const imageNumber = this.GetNumberFromImageMetaName(imageName);
    this.currentImageSrc = imageName;
    this.ScrollImagesPreviews(imageNumber);
  }

  GetNumberFromImageMetaName(name: string): number {
    return parseInt(name.split('-')[6]);
  }

  private ScrollImagesPreviews(imageNumber: number) {
    const previewImageWidth = 120;
    const element = document.getElementById('preview');
    element?.scroll({ left: imageNumber * previewImageWidth - 120 });
  }

  public AddToCart() {
    console.info('Clicked Add to cart button.');
  }
}
