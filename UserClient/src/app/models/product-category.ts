export interface ProductCategory {
    id: number;
    categoryName: string;
    parentCategory?: ProductCategory;
  }