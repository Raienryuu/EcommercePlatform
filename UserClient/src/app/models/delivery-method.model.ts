import { CustomerAddress } from './customer-address.model';

export interface DeliveryMethod {
  deliveryId: string;
  name: string;
  deliveryType: 'DeliveryPoint' | 'DirectCustomerAddress';
  paymentType: 'Cash' | 'Online';
  price: number;
  customerInformation: CustomerAddress | null;
}
