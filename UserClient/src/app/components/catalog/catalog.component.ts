import {Component, HostListener} from '@angular/core';
import {Product} from 'src/app/Models/product';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.scss'],
})


export class CatalogComponent {
  products: Product[] = [
    {id: 1, categoryId: 2, name: "Product A", description: "Description for Product A", price: 9.99, quantity: 10},
    {id: 2, categoryId: 3, name: "Product B", description: "Description for Product B", price: 19.99, quantity: 5},
    {id: 3, categoryId: 1, name: "Product C", description: "Description for Product C", price: 14.99, quantity: 8},
    {id: 4, categoryId: 2, name: "Product D", description: "Description for Product D", price: 24.99, quantity: 3},
    {id: 5, categoryId: 3, name: "Product E", description: "Description for Product E", price: 12.99, quantity: 12},
    {id: 6, categoryId: 1, name: "Product F", description: "Description for Product F", price: 29.99, quantity: 6},
    {id: 7, categoryId: 2, name: "Product G", description: "Description for Product G", price: 17.99, quantity: 9},
    {id: 8, categoryId: 3, name: "Product H", description: "Description for Product H", price: 21.99, quantity: 4},
    {id: 9, categoryId: 1, name: "Product I", description: "Description for Product I", price: 7.99, quantity: 15},
    {id: 10, categoryId: 2, name: "Product J", description: "Description for Product J", price: 11.99, quantity: 7},
  ];
  nameFilter: String = '';
  minPrice: number = 0;
  maxPrice: number = 0;

  categoryBreadcrumbs: string[] = ['Tupperware', 'Pots'];

  @HostListener('document:scroll', ['$event'])
  AppliedFiltersOpacityOnScroll() {
    const distance: number = 120;
    if (document.body.scrollTop > distance || document.documentElement.scrollTop > distance) {
      document.querySelector('div.applied-filters')!.classList.add('opacity');
    } else {
      document.querySelector('div.applied-filters')!.classList.remove('opacity');
    }
  }
}
