import { CustomerAddress } from './models/customer-address.model';
import { DeliveryMethod } from './models/delivery-method.model';
import { Product } from './models/product';

export const SampleProducts: Product[] = [
  {
    id: '931d05c4-993c-4d6d-b294-494f9bf27e24',
    categoryId: 3,
    name: 'Super long product name that will take multiple lines of text',
    description: 'Description for Product B',
    price: 19.99,
    quantity: 5,
  },
  {
    id: '69d2382e-66d7-4e85-b4a6-635a646e84ee',
    categoryId: 1,
    name: "Super long product name that will take multiple lines of text under the product's photo that is yet to be changed",
    description: 'Description for Product C',
    price: 14.99,
    quantity: 8,
  },
  {
    id: '3cbaa26a-80dc-4ef1-82c8-146ed0eba8e4',
    categoryId: 2,
    name: 'Short D',
    description: 'Description for Product D',
    price: 24.99,
    quantity: 1,
  },
];

export const SampleCustomerAddresses: CustomerAddress[] = [
  {
    id: 2,
    fullName: 'John California',
    address: '787 Dunbar Road',
    email: 'johnyboy@mail.com',
    phoneNumber: '+1 (213) 555-3890',
    city: 'San Jose, CA',
    zipCode: '95127',
    country: 'United States',
  },
  {
    id: 1,
    fullName: 'John Senior California',
    address: '788B Dunbar Road',
    email: 'oljohny@mail.com',
    phoneNumber: '+1 (213) 555-3890',
    city: 'San Jose, CA',
    zipCode: '95127',
    country: 'United States',
  },
];

export const SampleDeliveryMethod: DeliveryMethod[] = [
  {
    name: 'DHL Parcel Locker',
    deliveryId: '5',
    price: 5,
    deliveryType: 'DeliveryPoint',
    paymentType: 'Online',
    handlerName: 'dhl',
  },
];

export const SampleProduct = {
  name: 'WOW Cataclysm TROLL MAGE EU - PvP Giantstalker Level 30',
  id: 'f568479a-d723-43f3-b86d-7c51ca8e38c5',
  price: 88.32,
  quantity: 5,
  description: 'to be implmented',
  categoryId: 1,
};

export const SampleImageMetadata = {
  productId: 'f568479a-d723-43f3-b86d-7c51ca8e38c5',
  storedImages: ['p-12-0', 'p-12-1', 'p-12-2'],
};

export const LotsOfSampleProducts = [
  {
    id: '4728fd47-67ce-4104-846c-ef09fecb7036',
    categoryId: 2,
    name: 'Longer name Product A',
    description: 'Description for Product A',
    price: 9.99,
    quantity: 10,
  },
  {
    id: '7c96bec9-e3e4-4c78-bef2-e00ac8177844',
    categoryId: 3,
    name: 'Super long product name that will take multiple lines of text',
    description: 'Description for Product B',
    price: 19.99,
    quantity: 5,
  },
  {
    id: 'afdac98f-2457-427c-9ee0-3073dcf50bcf',
    categoryId: 1,
    name: "Super long product name that will take multiple lines of text under the product's photo that is yet to be changed",
    description: 'Description for Product C',
    price: 14.99,
    quantity: 8,
  },
  {
    id: '2500fb89-b210-4e5e-96b8-c0d219e36585',
    categoryId: 2,
    name: 'Short D',
    description: 'Description for Product D',
    price: 24.99,
    quantity: 3,
  },
  {
    id: '18b142b5-df6e-49d2-81f4-96706879a292',
    categoryId: 3,
    name: 'Product E',
    description: 'Description for Product E',
    price: 12.99,
    quantity: 12,
  },
  {
    id: '162240c3-d6f9-4b6f-85ff-8df9fdf84c6c',
    categoryId: 1,
    name: 'Product F',
    description: 'Description for Product F',
    price: 29.99,
    quantity: 6,
  },
  {
    id: 'dba22526-8468-466c-8f01-009178e082fe',
    categoryId: 2,
    name: 'Product G',
    description: 'Description for Product G',
    price: 17.99,
    quantity: 9,
  },
  {
    id: 'b2012c7a-bf55-4afa-8cd4-47d10abe407d',
    categoryId: 3,
    name: 'Product H',
    description: 'Description for Product H',
    price: 21.99,
    quantity: 4,
  },
  {
    id: 'ac0443f3-1684-4343-9131-9f686501825c',
    categoryId: 1,
    name: 'Product I',
    description: 'Description for Product I',
    price: 7.99,
    quantity: 15,
  },
  {
    id: '3be48d69-effb-4354-a16d-fda395486504',
    categoryId: 2,
    name: 'Product J',
    description: 'Description for Product J',
    price: 11.99,
    quantity: 7,
  },
];
