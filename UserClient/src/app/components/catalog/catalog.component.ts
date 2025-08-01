import { Component, EventEmitter, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductCategory } from 'src/app/models/product-category';
import { PaginationParams, SortType } from 'src/app/models/pagination-params';
import { ProductCategoryService } from 'src/app/services/productCategoryService/product-category.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { debounceTime } from 'rxjs';
import { environment } from 'src/enviroment';
import { CartService } from 'src/app/services/cartService/cart.service';
import { LotsOfSampleProducts } from 'src/app/develSamples';
import { IMAGE_LOADER, NgOptimizedImage } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { MatOptionModule } from '@angular/material/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { FormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { imageLoader } from 'src/app/images/imageLoader';

@Component({
    selector: 'app-catalog',
    templateUrl: './catalog.component.html',
    styleUrls: ['./catalog.component.scss'],
    imports: [
        MatProgressSpinnerModule,
        MatIconModule,
        MatFormFieldModule,
        MatOptionModule,
        MatSelectModule,
        NgOptimizedImage,
        MatSidenavModule,
        FormsModule,
        MatListModule,
        MatLabel,
        MatRadioModule,
        MatButtonModule,
        MatInputModule,
    ],
    providers: [{ provide: IMAGE_LOADER, useValue: imageLoader }]
})
export class ProductsComponent implements OnInit {
  products: Product[] = environment.sampleData ? LotsOfSampleProducts : [];

  filters: PaginationParams;
  oldFilters: PaginationParams;
  isLoading = true;
  maxPage = 10;
  filteringDelay = new EventEmitter();
  filterApplyDelay = new EventEmitter();
  public categoryId: string | null = '';
  currencySymbol = '€';

  categoryBreadcrumbs: string[] = ['Tupperware', 'Pots'];
  categoryChildren: ProductCategory[] = [
    { id: 5, categoryName: 'Arabic' },
    { id: 6, categoryName: 'Chinese' },
    { id: 7, categoryName: 'Japanese' },
  ];

  previewImageSource = '';

  constructor(
    private productService: ProductService,
    private productCategoryService: ProductCategoryService,
    private route: ActivatedRoute,
    private router: Router,
    private userSettingsService: UserSettingsService,
    private cartService: CartService,
  ) {
    this.filters = {
      PageNum: 1,
      PageSize: 5,
      Order: SortType.PriceAsc,
      Name: null!,
      MinPrice: null!,
      MaxPrice: null!,
      MinQuantity: null!,
      Categories: null!,
    };
    this.oldFilters = this.CloneParams(this.filters);
    this.filterApplyDelay
      .pipe(debounceTime(800))
      .subscribe(() => this.UpdateUrlQuery());

    this.filteringDelay.pipe(debounceTime(800)).subscribe(() => {
      this.LoadNewPage(0);
    });
  }

  private GetPageFromRoute(): number {
    const possiblePageNumber = parseInt(
      this.route.snapshot.queryParamMap.get('PageNum')!,
    );
    return possiblePageNumber ? possiblePageNumber : 1;
  }

  private GetPageSizeFromRoute(): number {
    const possiblePageSize = parseInt(
      this.route.snapshot.queryParamMap.get('PageSize')!,
    );
    return possiblePageSize ? possiblePageSize : 5;
  }

  ngOnInit() {
    this.LoadUserSettings();
    this.filters.PageNum = this.GetPageFromRoute();
    this.filters.PageSize = this.GetPageSizeFromRoute();
    this.GetCategoryTree();
    this.HookUpBackAndForwardButtons();

    if (environment.sampleData === true) {
      this.InsertNewProducts(LotsOfSampleProducts);
      return;
    }
  }

  private HookUpBackAndForwardButtons() {
    this.route.queryParams.subscribe(() => {
      this.filteringDelay.emit();
      this.filters.PageSize = this.GetPageSizeFromRoute();
      this.filters.PageNum = this.GetPageFromRoute();
      this.GetFiltersFromRoute();
    });
  }

  private GetFiltersFromRoute() {
    const minPrice = this.route.snapshot.queryParamMap.get('MinPrice');
    this.filters.MaxPrice = minPrice ? parseInt(minPrice) : null!;
    const maxPrice = this.route.snapshot.queryParamMap.get('MaxPrice');
    this.filters.MaxPrice = maxPrice ? parseInt(maxPrice) : null!;

    const name = this.route.snapshot.queryParamMap.get('Name') ?? '';
    if (name.length > 0) {
      this.filters.Name = name;
    } else {
      this.RemoveNameFilter();
    }
  }

  RefreshFilterDelay() {
    this.filterApplyDelay.emit();
  }

  HandleKeyWordsSearch() {
    this.ClearNameFilterIfEmpty();
    this.filteringDelay.emit();
  }

  private ClearNameFilterIfEmpty() {
    if (this.filters.Name.length < 1) {
      this.RemoveNameFilter();
    }
  }

  LoadUserSettings() {
    this.userSettingsService
      .GetCurrencySymbol()
      .subscribe((symbol) => (this.currencySymbol = symbol));
  }

  GetProductsPage(): void {
    this.productService.GetProductsPage(this.filters).subscribe((data) => {
      this.InsertNewProducts(data);
    });
  }

  private InsertNewProducts(data: Product[]) {
    this.products = [];
    this.products = data;
    this.isLoading = false;
  }

  GetNextPage(): void {
    this.productService
      .GetNextPage(this.filters, this.products[this.products.length - 1])
      .subscribe((data) => {
        this.InsertNewProducts(data);
      });
  }

  GetPreviousPage(): void {
    this.productService
      .GetPreviousPage(this.filters, this.products[0])
      .subscribe((data) => {
        this.InsertNewProducts(data);
      });
  }

  private UpdateUrlQuery() {
    if (this.AreParamsUnchanged()) {
      return;
    }

    this.oldFilters = this.CloneParams(this.filters);
    this.router.navigate([], {
      queryParams: this.filters,
      relativeTo: this.route,
      replaceUrl: false,
      skipLocationChange: false,
      queryParamsHandling: 'merge',
      onSameUrlNavigation: 'ignore',
    });
  }

  IsPageAvaiable(pageOffset: number): boolean {
    if (
      this.filters.PageNum + pageOffset > 0 &&
      this.filters.PageNum + pageOffset < this.maxPage
    )
      return true;
    return false;
  }

  /** pageOffset: -1 or 1 correspond to previousPage and nextPage respectively */
  LoadNewPage(pageOffset: number) {
    this.filters.PageNum += pageOffset;
    this.UpdateUrlQuery();
    if (pageOffset === 1) {
      this.GetNextPage();
    } else if (pageOffset === -1) {
      this.GetPreviousPage();
    } else {
      this.GetProductsPage();
    }
  }

  RemoveNameFilter() {
    this.filters.Name = null!;
    this.UpdateUrlQuery();
  }

  RemoveMinPriceFilter() {
    this.filters.MinPrice = null!;
    this.UpdateUrlQuery();
  }

  RemoveMaxPriceFilter() {
    this.filters.MaxPrice = null!;
    this.UpdateUrlQuery();
  }

  GetCategoryTree() {
    if (this.categoryId)
      this.productCategoryService
        .GetCategoryChildren(this.categoryId)
        .subscribe((categories) => (this.categoryChildren = categories));
  }

  @HostListener('document:scroll', ['$event'])
  AppliedFiltersOpacityOnScroll() {
    const distance = 120;
    if (
      document.body.scrollTop > distance ||
      document.documentElement.scrollTop > distance
    ) {
      document.querySelector('div.applied-filters')!.classList.add('opacity');
    } else {
      document
        .querySelector('div.applied-filters')!
        .classList.remove('opacity');
    }
  }

  UpdatePageSize(event: MatSelectChange) {
    this.filters.PageSize = event.value;
    this.filteringDelay.emit();
  }

  AddToCart(productId: string) {
    this.cartService.AddToCart(productId, 1);
  }

  GetPreviewImageSource(productId: string): string {
    return 'p-' + productId + '-0';
  }

  AreParamsUnchanged(): boolean {
    if (this.filters.Name !== this.oldFilters.Name) return false;
    if (this.filters.Order !== this.oldFilters.Order) return false;
    if (this.filters.PageNum !== this.oldFilters.PageNum) return false;
    if (this.filters.MaxPrice !== this.oldFilters.MaxPrice) return false;
    if (this.filters.MinPrice !== this.oldFilters.MinPrice) return false;
    if (this.filters.PageSize !== this.oldFilters.PageSize) return false;
    if (this.filters.Categories !== this.oldFilters.Categories) return false;
    if (this.filters.MinQuantity !== this.oldFilters.MinQuantity) return false;

    return true;
  }

  private CloneParams(paramsToClone: PaginationParams): PaginationParams {
    return {
      Name: paramsToClone.Name,
      Order: paramsToClone.Order,
      PageNum: paramsToClone.PageNum,
      MaxPrice: paramsToClone.MaxPrice,
      MinPrice: paramsToClone.MinPrice,
      PageSize: paramsToClone.PageSize,
      Categories: paramsToClone.Categories,
      MinQuantity: paramsToClone.MinQuantity,
    };
  }

  NavigateToDetails(productId: string) {
    this.router.navigate([`details/${productId}`]);
  }
}
