import { Component, EventEmitter, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductCategory } from 'src/app/models/product-category';
import { PaginationParams, SortType } from 'src/app/models/pagination-params';
import { ProductCategoryService } from 'src/app/services/productCategoryService/product-category.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
import { MatSelectChange } from '@angular/material/select';
import { debounceTime } from 'rxjs';
import { environment } from 'src/enviroment';
import { CartService } from 'src/app/services/cartService/cart.service';
import { LotsOfSampleProducts } from 'src/app/develSamples';

@Component({
  selector: 'app-catalog',
  standalone: false,
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.scss'],
})
export class ProductsComponent implements OnInit {


  products: Product[] = environment.sampleData ? LotsOfSampleProducts : [];

  filters: PaginationParams;
  isLoading = true;
  maxPage = 10;
  filteringDelay = new EventEmitter();
  public categoryId: string | null = '';
  currencySymbol = '€';

  categoryBreadcrumbs: string[] = ['Tupperware', 'Pots'];
  categoryChildren: ProductCategory[] = [
    { id: 5, categoryName: 'Arabic' },
    { id: 6, categoryName: 'Chinese' },
    { id: 7, categoryName: 'Japanese' },
  ];

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

    this.filteringDelay
      .pipe(debounceTime(800))
      .subscribe(() => {
        this.UpdateUrlQuery();
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
    this.GetProductsPage();
  }

  private HookUpBackAndForwardButtons() {
    this.route.queryParams.subscribe(() => {
      this.filters.PageSize = this.GetPageSizeFromRoute();
      this.filters.PageNum = this.GetPageFromRoute();
      this.GetFiltersFromRoute();
      this.LoadNewPage(0);
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
    this.filteringDelay.emit();
  }

  HandleKeyWordsSearch() {
    this.filteringDelay.emit();
    this.ClearNameFilterIfEmpty();
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
    this.router.navigate([], {
      queryParams: this.filters,
      relativeTo: this.route,
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

  LoadNewPage(pageOffset: number) {
    this.filters.PageNum += pageOffset;
    if (pageOffset === 1) {
      this.GetNextPage();
    } else if (pageOffset === -1) {
      this.GetPreviousPage();
    } else {
      this.GetProductsPage();
    }
    this.UpdateUrlQuery();
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

  AddToCart(productId: number) {
    this.cartService.AddToCart(productId, 1).subscribe(cartId =>{ this.cartService.remoteCartId = cartId; console.log("cart was updated")});
  }
}
