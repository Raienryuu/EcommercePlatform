import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Product } from 'src/app/models/product';
import { ProductCategory } from 'src/app/models/product-category';
import { SearchFilters, SortType } from 'src/app/models/search-filters';
import { ProductCategoryService } from 'src/app/services/productCategoryService/product-category.service';
import { ProductService } from 'src/app/services/productService/product.service';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.scss'],
})
export class CatalogComponent implements OnInit {
  products: Product[] = [
    {
      id: 1,
      categoryId: 2,
      name: 'Longer name Product A',
      description: 'Description for Product A',
      price: 9.99,
      quantity: 10,
    },
    {
      id: 2,
      categoryId: 3,
      name: 'Super long product name that will take multiple lines of text',
      description: 'Description for Product B',
      price: 19.99,
      quantity: 5,
    },
    {
      id: 3,
      categoryId: 1,
      name: "Super long product name that will take multiple lines of text under the product's photo that is yet to be changed",
      description: 'Description for Product C',
      price: 14.99,
      quantity: 8,
    },
    {
      id: 4,
      categoryId: 2,
      name: 'Short D',
      description: 'Description for Product D',
      price: 24.99,
      quantity: 3,
    },
    {
      id: 5,
      categoryId: 3,
      name: 'Product E',
      description: 'Description for Product E',
      price: 12.99,
      quantity: 12,
    },
    {
      id: 6,
      categoryId: 1,
      name: 'Product F',
      description: 'Description for Product F',
      price: 29.99,
      quantity: 6,
    },
    {
      id: 7,
      categoryId: 2,
      name: 'Product G',
      description: 'Description for Product G',
      price: 17.99,
      quantity: 9,
    },
    {
      id: 8,
      categoryId: 3,
      name: 'Product H',
      description: 'Description for Product H',
      price: 21.99,
      quantity: 4,
    },
    {
      id: 9,
      categoryId: 1,
      name: 'Product I',
      description: 'Description for Product I',
      price: 7.99,
      quantity: 15,
    },
    {
      id: 10,
      categoryId: 2,
      name: 'Product J',
      description: 'Description for Product J',
      price: 11.99,
      quantity: 7,
    },
  ];

  filters: SearchFilters;

  pageNum = 1;
  pageSize = 2;
  maxPage = 10;
  public categoryId: string | null;
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
    private userSettingsService: UserSettingsService,
  ) {
    this.filters = {
      Order: SortType.PriceAsc,
      Name: '',
      MinPrice: null!,
      MaxPrice: null!,
      MinQuantity: null!,
      Categories: null!,
    };
    this.categoryId = this.route.snapshot.paramMap.get('categoryId');
  }

  ngOnInit() {
    this.LoadUserSettings();
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
      .GetProductsPage(this.pageNum + pageOffset, this.pageSize, this.filters)
      .subscribe((data) => (this.products = data));
  }

  GetNextPage(): void {
    this.productService
      .GetNextPage(
        this.pageSize,
        this.filters,
        this.products[this.products.length - 1],
      )
      .subscribe((data) => (this.products = data));
  }

  GetPreviousPage(): void {
    this.productService
      .GetPreviousPage(this.pageSize, this.filters, this.products[0])
      .subscribe((data) => (this.products = data));
  }

  IsPageAvaiable(pageOffset: number): boolean {
    if (
      this.pageNum + pageOffset > 0 &&
      this.pageNum + pageOffset < this.maxPage
    )
      return true;
    return false;
  }

  LoadNewPage(pageOffset: number) {
    if (pageOffset === 1) {
      this.GetNextPage();
    } else if (pageOffset === -1) {
      this.GetPreviousPage();
    } else {
      this.GetProductsPage(pageOffset);
    }
    this.products = [];
    this.pageNum += pageOffset;
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
}
