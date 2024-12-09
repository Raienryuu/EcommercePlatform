import {
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductCategory } from 'src/app/models/product-category';
import { PaginationParams, SortType } from 'src/app/models/pagination-params';
import { ProductCategoryService } from 'src/app/services/productCategoryService/product-category.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';
import { MatSelectChange } from '@angular/material/select';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.scss'],
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];

  filters: PaginationParams;

  maxPage = 10;
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
    private changeDetector: ChangeDetectorRef,
  ) {
    setInterval(() => console.log(this.filters.PageNum), 1000);

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
    this.GetProductsPage(0);
    this.GetCategoryTree();
  }

  LoadUserSettings() {
    this.userSettingsService
      .GetCurrencySymbol()
      .subscribe((symbol) => (this.currencySymbol = symbol));
  }

  GetProductsPage(pageOffset: number): void {
    this.productService
      .GetProductsPage(
        this.filters.PageNum + pageOffset,
        this.filters.PageSize,
        this.filters,
      )
      .subscribe((data) => (this.products = data));
  }

  GetNextPage(): void {
    this.productService
      .GetNextPage(
        this.filters.PageSize,
        this.filters,
        this.products[this.products.length - 1],
      )
      .subscribe((data) => (this.products = data));
  }

  private UpdateUrlQuery() {
    this.router.navigate([], {
      queryParams: this.filters,
      relativeTo: this.route,
      queryParamsHandling: 'merge',
      onSameUrlNavigation: 'reload',
    });
  }

  GetPreviousPage(): void {
    this.productService
      .GetPreviousPage(this.filters.PageSize, this.filters, this.products[0])
      .subscribe((data) => (this.products = data));
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
    this.products = [];
    if (pageOffset === 1) {
      this.GetNextPage();
    } else if (pageOffset === -1) {
      this.GetPreviousPage();
    } else {
      this.GetProductsPage(pageOffset);
    }
    this.filters.PageNum += pageOffset;
    this.UpdateUrlQuery();
  }

  RemoveNameFilter() {
    this.filters.Name = '';
  }

  RemoveMinPriceFilter() {
    this.filters.MinPrice = null!;
  }

  RemoveMaxPriceFilter() {
    this.filters.MaxPrice = null!;
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

  @HostListener('window:popstate', ['$event'])
  onPopState() {
    this.route.queryParams.subscribe(() => {
      this.filters.PageSize = this.GetPageSizeFromRoute();
      this.filters.PageNum = this.GetPageFromRoute();
    });
  }

  UpdatePageSize(event: MatSelectChange) {
    this.filters.PageSize = event.value;
  }
}
