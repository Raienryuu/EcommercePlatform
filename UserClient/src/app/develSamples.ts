import { CustomerAddress } from './models/customer-address.model';
import { Product } from './models/product'

export const SampleProducts: Product[] = [
  { id: 2, categoryId: 3, name: "Super long product name that will take multiple lines of text", description: "Description for Product B", price: 19.99, quantity: 5 },
  { id: 3, categoryId: 1, name: "Super long product name that will take multiple lines of text under the product's photo that is yet to be changed", description: "Description for Product C", price: 14.99, quantity: 8 },
  { id: 4, categoryId: 2, name: "Short D", description: "Description for Product D", price: 24.99, quantity: 3 },
];


export const SampleCustomerAddresses: CustomerAddress[] = [
  {
    Id: 2,
    FullName: 'John California',
    Address: '787 Dunbar Road',
    Email: 'johnyboy@mail.com',
    PhoneNumber: '+1 (213) 555-3890',
    City: 'San Jose, CA',
    ZIPCode: '95127',
    Country: 'United States',
  },
  {
    Id: 1,
    FullName: 'John Senior California',
    Address: '788B Dunbar Road',
    Email: 'oljohny@mail.com',
    PhoneNumber: '+1 (213) 555-3890',
    City: 'San Jose, CA',
    ZIPCode: '95127',
    Country: 'United States',
  },
];

export const SampleProduct = {
  name: "WOW Cataclysm TROLL MAGE EU - PvP Giantstalker Level 30",
  id: 1,
  price: 88.32,
  quantity: 5,
  description: "to be implmented",
  categoryId: 1
};

export const SampleImageMetadata = {
  productId: 1,
  storedImages: ["p-12-0", "p-12-1", "p-12-2"],
}

export const LotsOfSampleProducts = [
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

